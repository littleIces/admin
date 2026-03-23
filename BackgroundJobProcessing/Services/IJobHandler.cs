namespace BackgroundJobProcessing.Services;

public interface IJobHandler
{
    string JobType { get; }
    Task<string> HandleAsync(string? payload, CancellationToken cancellationToken);
}
