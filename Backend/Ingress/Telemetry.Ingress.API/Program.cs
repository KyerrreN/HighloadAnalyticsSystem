using Asp.Versioning;
using Telemetry.Ingress.API.Infrastructure.DependencyInjectionExtensions;
using Telemetry.Ingress.API.Infrastructure.Endpoints;
using Telemetry.Ingress.API.Infrastructure.Exceptions;
using Telemetry.Ingress.API.Infrastructure.Observability.Otel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSwagger();
builder.Services.ConfigureCaching(builder.Configuration);
builder.Services.ConfigureGrpc(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.ConfigureAuthentication();
builder.Services.AddAuthorization();

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
builder.Services.RegisterRocksDb(builder.Configuration);

// could be overkill, but wouldn't hurt
builder.Services.Configure<HostOptions>(opt =>
{
    opt.ServicesStartConcurrently = true;
    opt.ServicesStopConcurrently = true;
});

builder.Services.AddSingleton<IngressMetrics>();
builder.Services.AddSingleton(TimeProvider.System);

builder.ConfigureOpenTelemetry();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpLogging();

// Request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();
