using CadViewer.Application.Interfaces;
using CadViewer.Domain.Entities;
using CadViewer.Domain.Interfaces;
using CadViewer.Domain.ValueObjects;

namespace CadViewer.Application.Services
{
	public class ModelProcessingService : IModelProcessingService
	{
		private readonly IModelStorage _storage;
		private readonly IJobStore _jobs;
		private readonly IBackgroundTaskQueue _queue;

		public ModelProcessingService(IModelStorage storage, IJobStore jobs, IBackgroundTaskQueue queue)
		{
			_storage = storage;
			_jobs = jobs;
			_queue = queue;
		}

		public async Task<ModelId> EnqueueUploadAsync(Stream stream, string fileName, CancellationToken cancellationToken)
		{
			var id = ModelId.New();
			await _storage.StoreSourceAsync(id, stream, fileName, cancellationToken);
			var job = new ModelJob(id, fileName);
			await _jobs.CreateAsync(job, cancellationToken);
			await _queue.EnqueueAsync(id, cancellationToken);
			return id;
		}

		public Task<ModelJob?> GetStatusAsync(ModelId id, CancellationToken cancellationToken)
			=> _jobs.GetAsync(id, cancellationToken);

		public Task<Stream> GetGltfAsync(ModelId id, CancellationToken cancellationToken)
			=> _storage.OpenGltfAsync(id, cancellationToken);

		public Task<string> GetSvgAsync(ModelId id, CancellationToken cancellationToken)
			=> _storage.ReadSvgAsync(id, cancellationToken);

		public async Task DeleteAsync(ModelId id, CancellationToken cancellationToken)
		{
			await _storage.DeleteAsync(id, cancellationToken);
			await _jobs.DeleteAsync(id, cancellationToken);
		}
	}
}