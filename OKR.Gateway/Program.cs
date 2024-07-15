using Microsoft.Extensions.Options;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OKR.Gateway;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("ocelot.json")
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .Build(); ;

var configurationAppSetting = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configurationAppSetting)
    //.Enrich.FromLogContext()
    .CreateLogger();

//builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddOcelot(configuration);
builder.Services.AddSwaggerForOcelot(configuration);
var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerForOcelotUI();
//}
//app.UseCors(builder => builder.WithOrigins("https://localhost:7231/").AllowAnyHeader().AllowAnyMethod());
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseOcelot().Wait();

//app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();

app.MapControllers();

app.Run();
