using Telemetry.Read.API.Infrastructure.Extensions;
using Telemetry.Read.API.Infrastructure.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterOptions(builder.Configuration);
builder.Services.AddDistributedMemoryCache();

builder.Services.AddQueryPipeline(typeof(Program).Assembly);

builder.Services.RegisterCustomServices();
builder.Services.AddSingleton(TimeProvider.System);
builder.ConfigureOtel();

var app = builder.Build();

// Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapEndpoints(typeof(Program).Assembly);

app.Run();
