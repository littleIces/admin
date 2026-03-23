using BackgroundJobProcessing.Models;
using BackgroundJobProcessing.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundJobProcessing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly JobQueue _queue;

    public JobsController(JobQueue queue)
    {
        _queue = queue;
    }

    [HttpPost]
    public ActionResult<JobResponse> CreateJob([FromBody] CreateJobRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Type))
            return BadRequest(new { Error = "Job type is required" });

        var job = _queue.Enqueue(request);
        return CreatedAtAction(nameof(GetJob), new { id = job.Id }, JobResponse.FromJob(job));
    }

    [HttpGet("{id:guid}")]
    public ActionResult<JobResponse> GetJob(Guid id)
    {
        var job = _queue.GetJob(id);
        if (job == null)
            return NotFound(new { Error = $"Job {id} not found" });

        return Ok(JobResponse.FromJob(job));
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<JobResponse>> ListJobs([FromQuery] JobStatus? status = null)
    {
        var jobs = _queue.GetAllJobs(status);
        return Ok(jobs.Select(JobResponse.FromJob).ToList());
    }

    [HttpDelete("{id:guid}")]
    public IActionResult CancelJob(Guid id)
    {
        var job = _queue.GetJob(id);
        if (job == null)
            return NotFound(new { Error = $"Job {id} not found" });

        if (!_queue.CancelJob(id))
            return Conflict(new { Error = "Job can only be cancelled when in Pending status" });

        return Ok(new { Message = $"Job {id} cancelled" });
    }

    [HttpGet("stats")]
    public ActionResult<JobQueueStats> GetStats()
    {
        return Ok(_queue.GetStats());
    }
}
