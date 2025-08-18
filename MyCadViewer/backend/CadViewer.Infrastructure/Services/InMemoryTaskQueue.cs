using System.Collections.Concurrent;
using CadViewer.Domain.Interfaces;
using CadViewer.Domain.ValueObjects;

namespace CadViewer.Infrastructure.Services
{
	public class InMemoryTaskQueue : IBackgroundTaskQueue
	{
		private readonly Channel<ModelId> _channel = Channel.CreateUnbounded<ModelId>();

		public ValueTask EnqueueAsync(ModelId id, CancellationToken cancellationToken)
		{
			return _channel.Writer.WriteAsync(id, cancellationToken);
		}

		public async ValueTask<ModelId> DequeueAsync(CancellationToken cancellationToken)
		{
			return await _channel.Reader.ReadAsync(cancellationToken);
		}
	}
}