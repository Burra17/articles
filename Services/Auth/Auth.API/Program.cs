using Auth.API;
using Auth.API.Features.Persons;
using Auth.Application;
using Auth.Persistence;
using Auth.Persistence.Data.Test;
using Blocks.AspNetCore.Middlewares;
using Blocks.EntityFrameworkCore;
using FastEndpoints;
using FastEndpoints.Swagger;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ConfigureApiOptions(builder.Configuration);

#region Add Services
builder.Services
    .AddApiServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddPersistenceServices(builder.Configuration);

#endregion

var app = builder.Build();

#region InitData
app.Migrate<AuthDBContext>();
if (app.Environment.IsDevelopment())
{
    try { app.SeedTestData(); }
    catch (Exception ex) { Console.WriteLine($"[Seed] Skipped: {ex.Message}"); }
}
#endregion

#region Use
app
    .UseMiddleware<GlobalExceptionMiddleware>()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(c =>
    {
        c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
    })
    .UseSwaggerGen();

app.MapGrpcService<PersonGrpcService>();
#endregion

app.Run();
