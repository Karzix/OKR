using ConsumerWeightUpdate;
using ConsumerWeightUpdate.Repository;
using Microsoft.EntityFrameworkCore;
using OKR.Infrastructure;
using OKR.Models.Context;
using SharedSettings;
using System.Runtime.ConstrainedExecution;

var builder = WebApplication.CreateBuilder(args);
var sharedConfig = SharedConfig.LoadSharedConfiguration();
builder.Configuration.AddConfiguration(sharedConfig);
// Add services to the container.
builder.Services.AddDbContext<OKRDBContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    ));
builder.Services.AddScoped<IObjectivesRepository, ObjectivesRepository>();
builder.Services.AddScoped<IKeyResultRepository, KeyResultRepository>();
builder.Services.AddScoped<IProgressUpdatesRepository, ProgressUpdatesRepository>();
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddHostedService<RabbitMQConsumer>();

var app = builder.Build();

app.Run();
