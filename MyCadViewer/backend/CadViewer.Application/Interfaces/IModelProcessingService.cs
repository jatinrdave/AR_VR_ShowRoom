using CadViewer.Domain.ValueObjects;
using CadViewer.Domain.Entities;

namespace CadViewer.Application.Interfaces
{
	public interface IModelProcessingService
	{
		Task<ModelId> EnqueueUploadAsync(Stream stream, string fileName, CancellationToken cancellationToken);
		Task<Entities.ModelJob?> GetStatusAsync(ModelId id, CancellationToken cancellationToken);
		Task<Stream> GetGltfAsync(ModelId id, CancellationToken cancellationToken);
		Task<string> GetSvgAsync(ModelId id, CancellationToken cancellationToken);
		Task DeleteAsync(ModelId id, CancellationToken cancellationToken);
	}
}