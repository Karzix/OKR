using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OKR.API.StartUp;
using OKR.Infrastructure;
using OKR.Models.Context;
using OKR.Models.Entity;
using OKR.Service.Implementation;
using OKR.Service.Mapper;
using RabbitMQ.Client;
using Serilog;
using SharedSettings;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using static Quartz.Logging.OperationName;

var builder = WebApplication.CreateBuilder(args);
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var sharedConfig = SharedConfig.LoadSharedConfiguration(environment);
builder.Configuration.AddConfiguration(sharedConfig);

// Thêm các dịch vụ vào container.
builder.Services.AddControllers();
builder.Services.AddDbContext<OKRDBContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    ));

new ServiceRepoMapping().Mapping(builder);
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<UserManager<ApplicationUser>, CustomUserManager>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    //.Enrich.FromLogContext()
    .CreateLogger();

// Tìm hiểu thêm về việc cấu hình Swagger/OpenAPI tại https://aka.ms/aspnetcore/swashbuckle
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
    .AddRoles<IdentityRole>() 
    .AddEntityFrameworkStores<OKRDBContext>()
    .AddApiEndpoints();

// SignalR
//builder.Services.AddSignalR();

// RabbitMQ
//var rabbitMQConfig = builder.Configuration.GetSection("RabbitMQ");
//builder.Services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory()
//{
//    HostName = rabbitMQConfig["HostName"],
//    UserName = rabbitMQConfig["UserName"],
//    Password = rabbitMQConfig["Password"],
//    VirtualHost = rabbitMQConfig["VirtualHost"],
//    Port = int.Parse(rabbitMQConfig["Port"])
//});
//builder.Services.AddSingleton<RabbitMQ.Client.IConnection>(sp => sp.GetRequiredService<IConnectionFactory>().CreateConnection());
//builder.Services.AddSingleton<RabbitMQ.Client.IModel>(sp => sp.GetRequiredService<RabbitMQ.Client.IConnection>().CreateModel());

//  Cache
builder.Services.AddMemoryCache();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    var jobKey = new JobKey("SendReminderEmailJob");

    q.AddJob<SendReminderEmailJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SendReminderEmailJob-trigger")
        .WithCronSchedule("0 0 0 * * ?"));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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
//app.MapHub<WeightUpdateNotification>("/weightUpdateNotification");
app.Run();
