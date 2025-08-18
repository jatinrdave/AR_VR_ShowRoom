using CadViewer.Domain.Interfaces;
using CadViewer.Domain.ValueObjects;
using CadViewer.Domain.Entities;
using System.Text.Json;

namespace CadViewer.Infrastructure.Storage
{
	public class FileSystemModelStorage : IModelStorage
	{
		private readonly string _rootPath;

		public FileSystemModelStorage(string rootPath)
		{
			_rootPath = rootPath;
			Directory.CreateDirectory(_rootPath);
		}

		private string GetDir(ModelId id) => Path.Combine(_rootPath, id.ToString());
		private string SrcPath(ModelId id) => Path.Combine(GetDir(id), "source");
		private string GltfPath(ModelId id) => Path.Combine(GetDir(id), "model.glb");
		private string SvgPath(ModelId id) => Path.Combine(GetDir(id), "projection.svg");
		private string NamePath(ModelId id) => Path.Combine(GetDir(id), "name.txt");
		private string LayersPath(ModelId id) => Path.Combine(GetDir(id), "layers.json");

		public async Task StoreSourceAsync(ModelId id, Stream stream, string fileName, CancellationToken cancellationToken)
		{
			var dir = GetDir(id);
			Directory.CreateDirectory(dir);
			await using var file = File.Create(SrcPath(id));
			await stream.CopyToAsync(file, cancellationToken);
			await File.WriteAllTextAsync(NamePath(id), fileName, cancellationToken);
		}

		public Task<Stream> OpenSourceAsync(ModelId id, CancellationToken cancellationToken)
		{
			var s = (Stream)File.OpenRead(SrcPath(id));
			return Task.FromResult(s);
		}

		public Task StoreGltfAsync(ModelId id, byte[] data, CancellationToken cancellationToken)
			=> File.WriteAllBytesAsync(GltfPath(id), data, cancellationToken);

		public Task<Stream> OpenGltfAsync(ModelId id, CancellationToken cancellationToken)
		{
			var s = (Stream)File.OpenRead(GltfPath(id));
			return Task.FromResult(s);
		}

		public Task StoreSvgAsync(ModelId id, string svg, CancellationToken cancellationToken)
			=> File.WriteAllTextAsync(SvgPath(id), svg, cancellationToken);

		public Task<string> ReadSvgAsync(ModelId id, CancellationToken cancellationToken)
			=> File.ReadAllTextAsync(SvgPath(id), cancellationToken);

		public async Task StoreLayersAsync(ModelId id, IReadOnlyList<LayerInfo> layers, CancellationToken cancellationToken)
		{
			var json = JsonSerializer.Serialize(layers);
			await File.WriteAllTextAsync(LayersPath(id), json, cancellationToken);
		}

		public async Task<IReadOnlyList<LayerInfo>> ReadLayersAsync(ModelId id, CancellationToken cancellationToken)
		{
			var path = LayersPath(id);
			if (!File.Exists(path)) return Array.Empty<LayerInfo>();
			var json = await File.ReadAllTextAsync(path, cancellationToken);
			var items = JsonSerializer.Deserialize<List<LayerInfo>>(json) ?? new List<LayerInfo>();
			return items;
		}

		public Task DeleteAsync(ModelId id, CancellationToken cancellationToken)
		{
			var dir = GetDir(id);
			if (Directory.Exists(dir)) Directory.Delete(dir, recursive: true);
			return Task.CompletedTask;
		}
	}
}