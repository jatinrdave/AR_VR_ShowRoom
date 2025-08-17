using CadViewer.Domain.ValueObjects;
using CadViewer.Domain.Entities;

namespace CadViewer.Domain.Interfaces
{
    public interface ICadParser
    {
        Task<CadModel> ParseAsync(Stream fileStream, string fileName, CancellationToken cancellationToken);
        bool CanParse(string fileName);
    }

    public interface IModelConverter
    {
        Task<byte[]> ConvertToGltfAsync(CadModel model, CancellationToken cancellationToken);
        Task<string> ConvertToSvgAsync(CadModel model, CancellationToken cancellationToken);
    }

    public interface IModelStorage
    {
        Task StoreSourceAsync(ModelId id, Stream stream, string fileName, CancellationToken cancellationToken);
        Task<Stream> OpenSourceAsync(ModelId id, CancellationToken cancellationToken);
        Task StoreGltfAsync(ModelId id, byte[] data, CancellationToken cancellationToken);
        Task<Stream> OpenGltfAsync(ModelId id, CancellationToken cancellationToken);
        Task StoreSvgAsync(ModelId id, string svg, CancellationToken cancellationToken);
        Task<string> ReadSvgAsync(ModelId id, CancellationToken cancellationToken);
        Task StoreLayersAsync(ModelId id, IReadOnlyList<LayerInfo> layers, CancellationToken cancellationToken);
        Task<IReadOnlyList<LayerInfo>> ReadLayersAsync(ModelId id, CancellationToken cancellationToken);
        Task DeleteAsync(ModelId id, CancellationToken cancellationToken);
    }

    public interface IJobStore
    {
        Task CreateAsync(Entities.ModelJob job, CancellationToken cancellationToken);
        Task<Entities.ModelJob?> GetAsync(ModelId id, CancellationToken cancellationToken);
        Task UpdateAsync(Entities.ModelJob job, CancellationToken cancellationToken);
        Task DeleteAsync(ModelId id, CancellationToken cancellationToken);
    }

    public interface IBackgroundTaskQueue
    {
        ValueTask EnqueueAsync(ModelId id, CancellationToken cancellationToken);
        ValueTask<ModelId> DequeueAsync(CancellationToken cancellationToken);
    }
}