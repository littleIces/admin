using BackgroundJobProcessing.Services;
using BackgroundJobProcessing.Services.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<JobQueue>();
builder.Services.AddScoped<IJobHandler, EmailJobHandler>();
builder.Services.AddScoped<IJobHandler, DataExportJobHandler>();
builder.Services.AddScoped<IJobHandler, ReportGenerationJobHandler>();
builder.Services.AddHostedService<JobProcessorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
