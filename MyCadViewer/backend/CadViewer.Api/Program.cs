using CadViewer.Application.Interfaces;
using CadViewer.Application.Services;
using CadViewer.Domain.Interfaces;
using CadViewer.Domain.ValueObjects;
using CadViewer.Infrastructure.Converters;
using CadViewer.Infrastructure.Parsers;
using CadViewer.Infrastructure.Services;
using CadViewer.Infrastructure.Storage;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Serilog;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
	.ReadFrom.Configuration(context.Configuration)
	.ReadFrom.Services(services)
	.Enrich.FromLogContext()
	.WriteTo.Console());

builder.Services.AddResponseCompression(options =>
{
	options.EnableForHttps = true;
	options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
	options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<FormOptions>(o =>
{
	o.MultipartBodyLengthLimit = 100L * 1024L * 1024L; // 100MB
});

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
		policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(_ => true));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "CadViewer API", Version = "v1" });
});

// DI registrations
builder.Services.AddSingleton<IJobStore, InMemoryJobStore>();
builder.Services.AddSingleton<IBackgroundTaskQueue, InMemoryTaskQueue>();

builder.Services.AddSingleton<ICadParser>(sp => new CompositeCadParser(new ICadParser[]
{
	new DxfParser(),
	new DwgParser()
}));

builder.Services.AddSingleton<IModelConverter, BasicGltfConverter>();

var storageRoot = Path.Combine(AppContext.BaseDirectory, "data");
Directory.CreateDirectory(storageRoot);
builder.Services.AddSingleton<IModelStorage>(sp => new FileSystemModelStorage(storageRoot));

builder.Services.AddSingleton<IModelProcessingService, ModelProcessingService>();

builder.Services.AddHostedService<CadProcessingWorker>();

var app = builder.Build();

app.UseResponseCompression();
app.UseSerilogRequestLogging();
app.UseCors();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/error");
}

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapPost("/api/models", async (HttpRequest request, IModelProcessingService service, CancellationToken ct) =>
{
	if (!request.HasFormContentType) return Results.BadRequest("multipart form required");
	var form = await request.ReadFormAsync(ct);
	var file = form.Files.GetFile("file");
	if (file == null || file.Length == 0) return Results.BadRequest("file required");
	await using var stream = file.OpenReadStream();
	var id = await service.EnqueueUploadAsync(stream, file.FileName, ct);
	return Results.Accepted($"/api/models/{id}", new { id = id.ToString() });
});

app.MapGet("/api/models/{id}", async (string id, IModelProcessingService service, CancellationToken ct) =>
{
	if (!ModelId.TryParse(id, out var mid)) return Results.NotFound();
	var job = await service.GetStatusAsync(mid, ct);
	return job == null ? Results.NotFound() : Results.Ok(new { id, state = job.State.ToString(), error = job.ErrorMessage });
});

app.MapGet("/api/models/{id}/gltf", async (string id, IModelProcessingService service, IModelStorage storage, HttpResponse res, CancellationToken ct) =>
{
	if (!ModelId.TryParse(id, out var mid)) return Results.NotFound();
	res.Headers.CacheControl = "public, max-age=604800"; // 7 days
	var stream = await service.GetGltfAsync(mid, ct);
	return Results.Stream(stream, "model/gltf-binary", enableRangeProcessing: true);
});

app.MapGet("/api/models/{id}/svg", async (string id, IModelProcessingService service, HttpResponse res, CancellationToken ct) =>
{
	if (!ModelId.TryParse(id, out var mid)) return Results.NotFound();
	res.Headers.CacheControl = "public, max-age=604800"; // 7 days
	var svg = await service.GetSvgAsync(mid, ct);
	return Results.Text(svg, "image/svg+xml");
});

app.MapGet("/api/layers/{id}", async (string id, IModelStorage storage, CancellationToken ct) =>
{
	if (!ModelId.TryParse(id, out var mid)) return Results.NotFound();
	var layers = await storage.ReadLayersAsync(mid, ct);
	return Results.Ok(layers.Select(l => new { name = l.Name, color = l.ColorHex, visible = l.IsVisible }));
});

app.MapDelete("/api/models/{id}", async (string id, IModelProcessingService service, CancellationToken ct) =>
{
	if (!ModelId.TryParse(id, out var mid)) return Results.NotFound();
	await service.DeleteAsync(mid, ct);
	return Results.NoContent();
});

app.Run();