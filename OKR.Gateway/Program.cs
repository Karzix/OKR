
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OKR.Gateway;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("ocelot.json")
    .Build(); ;
//if (environment == "Development")
//{
//    configuration = new ConfigurationBuilder()
//    .AddJsonFile("ocelotDev.json")
//    .Build();
//}
//else
//{
//configuration = new ConfigurationBuilder()
//.AddJsonFile("ocelot.json")
//.Build();
//}
var configurationAppSetting = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configurationAppSetting)
    //.Enrich.FromLogContext()
    .CreateLogger();

//builder.Host.UseSerilog();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseOcelot().Wait();

app.UseAuthorization();

app.MapControllers();

app.Run();
