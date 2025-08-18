using CadViewer.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CadViewer.Infrastructure.Services
{
	public class CadProcessingWorker : BackgroundService
	{
		private readonly IBackgroundTaskQueue _queue;
		private readonly IJobStore _jobs;
		private readonly ICadParser _parser;
		private readonly IModelConverter _converter;
		private readonly IModelStorage _storage;
		private readonly ILogger<CadProcessingWorker> _logger;

		public CadProcessingWorker(
			IBackgroundTaskQueue queue,
			IJobStore jobs,
			ICadParser parser,
			IModelConverter converter,
			IModelStorage storage,
			ILogger<CadProcessingWorker> logger)
		{
			_queue = queue;
			_jobs = jobs;
			_parser = parser;
			_converter = converter;
			_storage = storage;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("CadProcessingWorker started");
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var id = await _queue.DequeueAsync(stoppingToken);
					var job = await _jobs.GetAsync(id, stoppingToken);
					if (job == null) continue;
					job.MarkProcessing();
					await _jobs.UpdateAsync(job, stoppingToken);

					await using var source = await _storage.OpenSourceAsync(id, stoppingToken);
					var model = await _parser.ParseAsync(source, job.OriginalFileName, stoppingToken);
					var gltf = await _converter.ConvertToGltfAsync(model, stoppingToken);
					await _storage.StoreGltfAsync(id, gltf, stoppingToken);
					var svg = await _converter.ConvertToSvgAsync(model, stoppingToken);
					await _storage.StoreSvgAsync(id, svg, stoppingToken);
					await _storage.StoreLayersAsync(id, model.Layers.ToList(), stoppingToken);

					job.MarkCompleted();
					await _jobs.UpdateAsync(job, stoppingToken);
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error processing CAD job");
				}
			}
		}
	}
}