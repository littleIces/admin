using System.Text.Json;

namespace BackgroundJobProcessing.Services.Handlers;

public class ReportPayload
{
    public string ReportType { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class ReportGenerationJobHandler : IJobHandler
{
    public string JobType => "generate-report";
    private readonly ILogger<ReportGenerationJobHandler> _logger;

    public ReportGenerationJobHandler(ILogger<ReportGenerationJobHandler> logger)
    {
        _logger = logger;
    }

    public async Task<string> HandleAsync(string? payload, CancellationToken cancellationToken)
    {
        var request = payload != null
            ? JsonSerializer.Deserialize<ReportPayload>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            : null;

        if (request == null)
            throw new ArgumentException("Invalid report payload");

        _logger.LogInformation("Generating {ReportType} report", request.ReportType);

        for (int i = 1; i <= 5; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            _logger.LogInformation("Report generation progress: {Percent}%", i * 20);
        }

        var reportId = Guid.NewGuid().ToString("N")[..8];
        _logger.LogInformation("Report generated: {ReportId}", reportId);

        return JsonSerializer.Serialize(new
        {
            ReportId = reportId,
            ReportType = request.ReportType,
            GeneratedAt = DateTime.UtcNow,
            PageCount = 42
        });
    }
}
