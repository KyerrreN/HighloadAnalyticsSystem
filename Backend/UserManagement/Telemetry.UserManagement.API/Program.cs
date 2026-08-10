using Telemetry.UserManagement.API.Extensions;
using Telemetry.UserManagement.API.Middlewares;
using Telemetry.UserManagement.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.RegisterCustomServices();
builder.Services.RegisterOptions(builder.Configuration);
builder.Services.RegisterRefit();

builder.Services.AddInfrastructure(builder.Configuration);

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
