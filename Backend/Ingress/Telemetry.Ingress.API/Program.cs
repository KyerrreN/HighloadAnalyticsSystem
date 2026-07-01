using Asp.Versioning;
using Microsoft.AspNetCore.HttpLogging;
using Telemetry.Ingress.API.Infrastructure.DependencyInjectionExtensions;
using Telemetry.Ingress.API.Infrastructure.Endpoints;
using Telemetry.Ingress.API.Infrastructure.Observability.Otel;

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

builder.Services.RegisterOptions(builder.Configuration);
builder.Services.RegisterServices();

// could be overkill, but wouldn't hurt
builder.Services.Configure<HostOptions>(opt =>
{
    opt.ServicesStartConcurrently = true;
    opt.ServicesStopConcurrently = true;
});

builder.Services.AddSingleton<IngressMetrics>();

builder.ConfigureOpenTelemetry();

var app = builder.Build();

app.UseHttpLogging();

// Request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();
