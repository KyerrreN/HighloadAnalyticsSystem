using FluentValidation;
using Telemetry.UserManagement.API.Extensions;
using Telemetry.UserManagement.API.Middlewares;
using Telemetry.UserManagement.Infrastructure.DI;
using Telemetry.UserManagement.Infrastructure.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.RegisterCustomServices();
builder.Services.RegisterOptions();
builder.Services.RegisterRefit();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.ConfigureOtel();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseMiddleware<EnsureUserExistsMiddleware>();

app.UseAuthorization();

app.MapAllEndpoints();

app.Run();
