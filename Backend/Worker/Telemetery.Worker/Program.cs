using Telemetry.Worker.Infrastructure.Options;
using Telemetry.Worker.Infrastructure.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<KafkaOptions>()
    .Bind(builder.Configuration.GetSection(KafkaOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHostedService<ClickHouseSetupService>();
// todo: maybe let this service initialize kafka too. And maybe clickhouse. Maybe look into IaC
builder.Services.AddHostedService<KafkaConsumerWorker>();

var host = builder.Build();
host.Run();
