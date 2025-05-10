using Hellang.Middleware.ProblemDetails;
using Homework1;
using Homework1.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddHttpClient<IPostService, PostService>();
builder.Services.AddHttpClient<IUserService, UserService>(client =>
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

    loggerConfig
        .MinimumLevel.Debug()
        .WriteTo.File(logFileName, rollingInterval: RollingInterval.Infinite);
});


builder.Services.AddProblemDetails(options =>
{
    options.Map<DuplicateUserNameException>(ex => new ProblemDetails
    {
        Status = StatusCodes.Status400BadRequest,
        Title = "Duplicate user",
        Detail = ex.Message,
        Type = "https://httpstatuses.com/400"
    });

    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
});


var app = builder.Build();

app.UseProblemDetails();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
