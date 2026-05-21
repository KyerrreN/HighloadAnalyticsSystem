using Telemetry.Worker.Infrastructure.Options;
using Telemetry.Worker.Infrastructure.Data;
using Telemetry.Worker.Infrastructure.Data.Interfaces;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<KafkaOptions>()
    .Bind(builder.Configuration.GetSection(KafkaOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<BatchingOptions>()
    .Bind(builder.Configuration.GetSection(BatchingOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<ClickHouseOptions>()
    .Bind(builder.Configuration.GetSection(ClickHouseOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<ITelemetrySink, ClickHouseTelemetrySinc>();

builder.Services.AddHostedService<ClickHouseSetupService>();
// todo: maybe let this service initialize kafka too. And maybe clickhouse. Maybe look into IaC
builder.Services.AddHostedService<KafkaConsumerWorker>();

var host = builder.Build();
host.Run();
