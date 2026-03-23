using System.Text.Json;

namespace BackgroundJobProcessing.Services.Handlers;

public class DataExportPayload
{
    public string Format { get; set; } = "csv";
    public string DataSource { get; set; } = string.Empty;
    public Dictionary<string, string>? Filters { get; set; }
}

public class DataExportJobHandler : IJobHandler
{
    public string JobType => "data-export";
    private readonly ILogger<DataExportJobHandler> _logger;

    public DataExportJobHandler(ILogger<DataExportJobHandler> logger)
    {
        _logger = logger;
    }

    public async Task<string> HandleAsync(string? payload, CancellationToken cancellationToken)
    {
        var request = payload != null
            ? JsonSerializer.Deserialize<DataExportPayload>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            : null;

        if (request == null)
            throw new ArgumentException("Invalid data export payload");

        _logger.LogInformation("Starting data export from {DataSource} in {Format} format",
            request.DataSource, request.Format);

        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

        var fileName = $"export_{request.DataSource}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{request.Format}";
        _logger.LogInformation("Data export completed: {FileName}", fileName);

        return JsonSerializer.Serialize(new { FileName = fileName, RecordCount = 1250, SizeBytes = 524288 });
    }
}
