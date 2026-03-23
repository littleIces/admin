using System.Text.Json;

namespace BackgroundJobProcessing.Services.Handlers;

public class EmailJobPayload
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public class EmailJobHandler : IJobHandler
{
    public string JobType => "send-email";
    private readonly ILogger<EmailJobHandler> _logger;

    public EmailJobHandler(ILogger<EmailJobHandler> logger)
    {
        _logger = logger;
    }

    public async Task<string> HandleAsync(string? payload, CancellationToken cancellationToken)
    {
        var email = payload != null
            ? JsonSerializer.Deserialize<EmailJobPayload>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            : null;

        if (email == null)
            throw new ArgumentException("Invalid email payload");

        _logger.LogInformation("Sending email to {To} with subject '{Subject}'", email.To, email.Subject);

        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

        _logger.LogInformation("Email sent successfully to {To}", email.To);
        return $"Email sent to {email.To} at {DateTime.UtcNow:O}";
    }
}
