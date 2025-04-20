using Homework1;
using Homework1.Controllers;
using System.Text.Json;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<PostsController>();
builder.Services.AddHttpClient<UsersController>();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

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
    loggerConfig.WriteTo.File("logFolder/Log.txt", rollingInterval: RollingInterval.Day);
    loggerConfig.MinimumLevel.Debug();
});
var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Homework1.DuplicateUserNameException ex)
    {
        Log.Warning("Duplicate user name exception: {Message}", ex.Message);
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { message = "A user with the same name already exists." });
    }
    catch (Exception ex)
    {
        Log.Error("Unhandled exception: {Message}", ex.Message);
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { message = "An unexpected error occurred." });
    }
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
