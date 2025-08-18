using System.Collections.Concurrent;
using CadViewer.Domain.Entities;
using CadViewer.Domain.Interfaces;
using CadViewer.Domain.ValueObjects;

namespace CadViewer.Infrastructure.Services
{
	public class InMemoryJobStore : IJobStore
	{
		private readonly ConcurrentDictionary<Guid, ModelJob> _jobs = new();

		public Task CreateAsync(ModelJob job, CancellationToken cancellationToken)
		{
			_jobs[job.Id.Value] = job;
			return Task.CompletedTask;
		}

		public Task<ModelJob?> GetAsync(ModelId id, CancellationToken cancellationToken)
		{
			_jobs.TryGetValue(id.Value, out var job);
			return Task.FromResult<ModelJob?>(job);
		}

		public Task UpdateAsync(ModelJob job, CancellationToken cancellationToken)
		{
			_jobs[job.Id.Value] = job;
			return Task.CompletedTask;
		}

		public Task DeleteAsync(ModelId id, CancellationToken cancellationToken)
		{
			_jobs.TryRemove(id.Value, out _);
			return Task.CompletedTask;
		}
	}
}