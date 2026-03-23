using System.Collections.Concurrent;
using System.Threading.Channels;
using BackgroundJobProcessing.Models;

namespace BackgroundJobProcessing.Services;

public class JobQueue
{
    private readonly ConcurrentDictionary<Guid, BackgroundJob> _jobs = new();
    private readonly Channel<Guid> _channel = Channel.CreateUnbounded<Guid>(new UnboundedChannelOptions
    {
        SingleReader = false,
        SingleWriter = false
    });

    public BackgroundJob Enqueue(CreateJobRequest request)
    {
        var job = new BackgroundJob
        {
            Type = request.Type,
            Payload = request.Payload,
            Priority = request.Priority,
            MaxRetries = request.MaxRetries
        };

        _jobs[job.Id] = job;
        _channel.Writer.TryWrite(job.Id);
        return job;
    }

    public async ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _channel.Reader.ReadAsync(cancellationToken);
    }

    public BackgroundJob? GetJob(Guid id)
    {
        _jobs.TryGetValue(id, out var job);
        return job;
    }

    public IReadOnlyList<BackgroundJob> GetAllJobs(JobStatus? statusFilter = null)
    {
        var query = _jobs.Values.AsEnumerable();
        if (statusFilter.HasValue)
            query = query.Where(j => j.Status == statusFilter.Value);
        return query.OrderByDescending(j => j.Priority).ThenBy(j => j.CreatedAt).ToList();
    }

    public bool CancelJob(Guid id)
    {
        if (!_jobs.TryGetValue(id, out var job))
            return false;

        if (job.Status != JobStatus.Pending)
            return false;

        job.Status = JobStatus.Cancelled;
        job.CompletedAt = DateTime.UtcNow;
        return true;
    }

    public void ReEnqueue(Guid id)
    {
        if (_jobs.TryGetValue(id, out var job))
        {
            job.Status = JobStatus.Pending;
            _channel.Writer.TryWrite(id);
        }
    }

    public JobQueueStats GetStats()
    {
        var jobs = _jobs.Values.ToList();
        return new JobQueueStats
        {
            TotalJobs = jobs.Count,
            PendingJobs = jobs.Count(j => j.Status == JobStatus.Pending),
            RunningJobs = jobs.Count(j => j.Status == JobStatus.Running),
            CompletedJobs = jobs.Count(j => j.Status == JobStatus.Completed),
            FailedJobs = jobs.Count(j => j.Status == JobStatus.Failed),
            CancelledJobs = jobs.Count(j => j.Status == JobStatus.Cancelled)
        };
    }
}

public class JobQueueStats
{
    public int TotalJobs { get; set; }
    public int PendingJobs { get; set; }
    public int RunningJobs { get; set; }
    public int CompletedJobs { get; set; }
    public int FailedJobs { get; set; }
    public int CancelledJobs { get; set; }
}
