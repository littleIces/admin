using BackgroundJobProcessing.Models;

namespace BackgroundJobProcessing.Services;

public class JobProcessorService : BackgroundService
{
    private readonly JobQueue _queue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<JobProcessorService> _logger;
    private readonly int _maxConcurrency;

    public JobProcessorService(
        JobQueue queue,
        IServiceProvider serviceProvider,
        ILogger<JobProcessorService> logger,
        IConfiguration configuration)
    {
        _queue = queue;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _maxConcurrency = configuration.GetValue("JobProcessing:MaxConcurrency", 4);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Job processor started with max concurrency: {MaxConcurrency}", _maxConcurrency);

        using var semaphore = new SemaphoreSlim(_maxConcurrency);
        var runningTasks = new List<Task>();

        while (!stoppingToken.IsCancellationRequested)
        {
            await semaphore.WaitAsync(stoppingToken);

            Guid jobId;
            try
            {
                jobId = await _queue.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                semaphore.Release();
                break;
            }

            var task = Task.Run(async () =>
            {
                try
                {
                    await ProcessJobAsync(jobId, stoppingToken);
                }
                finally
                {
                    semaphore.Release();
                }
            }, stoppingToken);

            runningTasks.Add(task);
            runningTasks.RemoveAll(t => t.IsCompleted);
        }

        await Task.WhenAll(runningTasks);
        _logger.LogInformation("Job processor stopped");
    }

    private async Task ProcessJobAsync(Guid jobId, CancellationToken cancellationToken)
    {
        var job = _queue.GetJob(jobId);
        if (job == null || job.Status == JobStatus.Cancelled)
            return;

        job.Status = JobStatus.Running;
        job.StartedAt = DateTime.UtcNow;
        _logger.LogInformation("Processing job {JobId} of type {JobType}", job.Id, job.Type);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var handlers = scope.ServiceProvider.GetServices<IJobHandler>();
            var handler = handlers.FirstOrDefault(h =>
                h.JobType.Equals(job.Type, StringComparison.OrdinalIgnoreCase));

            if (handler == null)
            {
                job.Status = JobStatus.Failed;
                job.Error = $"No handler registered for job type '{job.Type}'";
                job.CompletedAt = DateTime.UtcNow;
                _logger.LogError("No handler found for job type {JobType}", job.Type);
                return;
            }

            var result = await handler.HandleAsync(job.Payload, cancellationToken);
            job.Status = JobStatus.Completed;
            job.Result = result;
            job.CompletedAt = DateTime.UtcNow;
            _logger.LogInformation("Job {JobId} completed successfully", job.Id);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            job.RetryCount++;
            _logger.LogError(ex, "Job {JobId} failed (attempt {Attempt}/{MaxRetries})",
                job.Id, job.RetryCount, job.MaxRetries);

            if (job.RetryCount < job.MaxRetries)
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, job.RetryCount));
                _logger.LogInformation("Retrying job {JobId} in {Delay}s", job.Id, delay.TotalSeconds);
                await Task.Delay(delay, cancellationToken);
                _queue.ReEnqueue(job.Id);
            }
            else
            {
                job.Status = JobStatus.Failed;
                job.Error = ex.Message;
                job.CompletedAt = DateTime.UtcNow;
                _logger.LogError("Job {JobId} permanently failed after {MaxRetries} retries", job.Id, job.MaxRetries);
            }
        }
    }
}
