using Blocks.AspNetCore.Filters;
using Blocks.AspNetCore.Middlewares;
using Submission.API;
using Submission.API.Endpoints;
using Submission.Application;
using Submission.Persistence;

var builder = WebApplication.CreateBuilder(args);
#region Add Services 
builder.Services
    .AddApiServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddPersistenceServices(builder.Configuration);

#endregion
var app = builder.Build();
#region Use Services
app
    .UseSwagger()
    .UseSwaggerUI()
    .UseRouting()           // Match the incoming request to an endpoint.
    .UseMiddleware<GlobalExceptionMiddleware>()
    ;

app.MapAllEndpoints();

app.MapGroup("/api").AddEndpointFilter<AssignUserIdFilter>();

// todo - migrate - create first migration
// 
if (app.Environment.IsDevelopment())
{
    // todo seed test data
}

#endregion


app.Run();