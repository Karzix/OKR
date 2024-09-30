using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OKR.API.StartUp;
using OKR.Infrastructure;
using OKR.Models.Context;
using OKR.Models.Entity;
using OKR.Service.Mapper;
using RabbitMQ.Client;
using Serilog;
using SharedSettings;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var sharedConfig = SharedConfig.LoadSharedConfiguration(environment);
builder.Configuration.AddConfiguration(sharedConfig);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<OKRDBContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    ));
new ServiceRepoMapping().Mapping(builder);
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    //.Enrich.FromLogContext()
    .CreateLogger();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.SetIsOriginAllowed(origin => true)
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
builder.Services.AddAuthorization();
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>() // Add roles services
    .AddEntityFrameworkStores<OKRDBContext>()
    .AddApiEndpoints();
//SignalR
builder.Services.AddSignalR();
//RabbitMQ
var rabbitMQConfig = builder.Configuration.GetSection("RabbitMQ");
builder.Services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory()
{
    HostName = rabbitMQConfig["HostName"],
    UserName = rabbitMQConfig["UserName"],
    Password = rabbitMQConfig["Password"],
    VirtualHost = rabbitMQConfig["VirtualHost"],
    Port = int.Parse(rabbitMQConfig["Port"])
});
builder.Services.AddSingleton<RabbitMQ.Client.IConnection>(sp => sp.GetRequiredService<IConnectionFactory>().CreateConnection());
builder.Services.AddSingleton<RabbitMQ.Client.IModel>(sp => sp.GetRequiredService<RabbitMQ.Client.IConnection>().CreateModel());
//MemoryCache 
builder.Services.AddMemoryCache();
var app = builder.Build();
app.UseRouting();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapIdentityApi<ApplicationUser>();
app.MapHub<WeightUpdateNotification>("/weightUpdateNotification");
app.Run();
