using ConsumerWeightUpdate;
using OKR.Infrastructure;
using SharedSettings;

var builder = WebApplication.CreateBuilder(args);
var sharedConfig = SharedConfig.LoadSharedConfiguration();
builder.Configuration.AddConfiguration(sharedConfig);
// Add services to the container.

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddHostedService<RabbitMQConsumer>();

var app = builder.Build();

app.Run();
