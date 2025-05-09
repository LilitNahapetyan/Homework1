using Homework1;
using Homework1.Controllers;
using Homework1.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<PostsController>();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddHttpClient<UsersController>(client =>
{
    client.DefaultRequestHeaders.Add("x-api-key", "reqres-free-v1");
});
builder.Services.AddSingleton(sp =>
{
    return new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };
});

builder.Host.UseSerilog((context, loggerConfig) =>
{
    var startOfWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + (int)DayOfWeek.Monday);
    var logFileName = $"logFolder/Log-Week-{startOfWeek:yyyy-MM-dd}.txt";

    loggerConfig.WriteTo.File(
        logFileName,
        rollingInterval: RollingInterval.Infinite
    );
});


var app = builder.Build();



app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionFeature?.Error;

        context.Response.ContentType = "application/problem+json";
        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path,
            Detail = exception?.Message
        };

        if (exception is DuplicateUserNameException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Duplicate user";
            problemDetails.Type = "https://httpstatuses.com/400";
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Title = "An unexpected error occurred.";
            problemDetails.Type = "https://httpstatuses.com/500";
        }

        Log.Error(exception, "Exception caught: {Message}", exception?.Message);
        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
