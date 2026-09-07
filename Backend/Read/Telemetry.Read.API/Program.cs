using Telemetry.Read.API.Infrastructure.Extensions;
using Telemetry.Read.API.Infrastructure.Observability;
using Telemetry.Shared.Caching;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSwagger();
builder.Services.RegisterOptions(builder.Configuration);
builder.Services.AddHybridCaching(builder.Configuration);
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();

builder.Services.AddQueryPipeline(typeof(Program).Assembly);

builder.Services.RegisterCustomServices();
builder.Services.AddSingleton(TimeProvider.System);
builder.ConfigureOtel();

var app = builder.Build();

// Request Pipeline
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints(typeof(Program).Assembly);

app.Run();
