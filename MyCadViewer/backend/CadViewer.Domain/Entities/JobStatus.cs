namespace CadViewer.Domain.Entities
{
    public enum ProcessingState
    {
        Unknown = 0,
        Queued = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4
    }

    public class ModelJob
    {
        public ModelJob(ValueObjects.ModelId id, string originalFileName)
        {
            Id = id;
            OriginalFileName = originalFileName;
            State = ProcessingState.Queued;
            CreatedUtc = DateTimeOffset.UtcNow;
        }

        public ValueObjects.ModelId Id { get; }

        public string OriginalFileName { get; }

        public ProcessingState State { get; private set; }

        public string? ErrorMessage { get; private set; }

        public DateTimeOffset CreatedUtc { get; }

        public DateTimeOffset? CompletedUtc { get; private set; }

        public void MarkProcessing()
        {
            State = ProcessingState.Processing;
        }

        public void MarkCompleted()
        {
            State = ProcessingState.Completed;
            CompletedUtc = DateTimeOffset.UtcNow;
        }

        public void MarkFailed(string error)
        {
            State = ProcessingState.Failed;
            ErrorMessage = error;
            CompletedUtc = DateTimeOffset.UtcNow;
        }
    }
}