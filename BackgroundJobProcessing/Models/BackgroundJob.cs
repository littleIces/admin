namespace BackgroundJobProcessing.Models;

public enum JobStatus
{
    Pending,
    Running,
    Completed,
    Failed,
    Cancelled
}

public enum JobPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Critical = 3
}

public class BackgroundJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public string? Payload { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public JobPriority Priority { get; set; } = JobPriority.Normal;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Result { get; set; }
    public string? Error { get; set; }
    public int RetryCount { get; set; }
    public int MaxRetries { get; set; } = 3;
}

public class CreateJobRequest
{
    public string Type { get; set; } = string.Empty;
    public string? Payload { get; set; }
    public JobPriority Priority { get; set; } = JobPriority.Normal;
    public int MaxRetries { get; set; } = 3;
}

public class JobResponse
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public JobStatus Status { get; set; }
    public JobPriority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Result { get; set; }
    public string? Error { get; set; }
    public int RetryCount { get; set; }
    public int MaxRetries { get; set; }

    public static JobResponse FromJob(BackgroundJob job) => new()
    {
        Id = job.Id,
        Type = job.Type,
        Status = job.Status,
        Priority = job.Priority,
        CreatedAt = job.CreatedAt,
        StartedAt = job.StartedAt,
        CompletedAt = job.CompletedAt,
        Result = job.Result,
        Error = job.Error,
        RetryCount = job.RetryCount,
        MaxRetries = job.MaxRetries
    };
}
