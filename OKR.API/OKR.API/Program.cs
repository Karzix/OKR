using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OKR.API.StartUp;
using OKR.Models.Context;
using OKR.Models.Entity;
using OKR.Service.Mapper;
using Serilog;
using SharedSettings;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var sharedConfig = SharedConfig.LoadSharedConfiguration();
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

builder.Services.AddAuthorization();
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>() // Add roles services
    .AddEntityFrameworkStores<OKRDBContext>()
    .AddApiEndpoints();

var app = builder.Build();
app.MapControllers();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapIdentityApi<ApplicationUser>();

app.Run();
