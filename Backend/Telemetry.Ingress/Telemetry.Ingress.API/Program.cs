using Asp.Versioning;
using Telemetry.Ingress.API.Infrastructure.Endpoints;
using Telemetry.Ingress.API.Infrastructure.MessageProcessing;
using Telemetry.Ingress.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// versioning
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1);
    opt.ReportApiVersions = true;
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'V";
    opt.SubstituteApiVersionInUrl = true;
});

// Custom services
builder.Services.AddSingleton<IEventMessageBus, KafkaEventMessageBus>();
builder.Services.AddSingleton<ITelemetryEventChannel, TelemetryEventChannel>();

builder.Services.AddHostedService<TelemetryPublishWorker>();

// could be overkill, but wouldn't hurt
builder.Services.Configure<HostOptions>(opt =>
{
    opt.ServicesStartConcurrently = true;
    opt.ServicesStopConcurrently = true;
});

var app = builder.Build();

// Request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();
