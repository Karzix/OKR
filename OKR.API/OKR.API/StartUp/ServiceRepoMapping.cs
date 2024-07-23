using OKR.Repository.Contract;
using OKR.Repository.Implementation;
using OKR.Service.Contract;
using OKR.Service.Implementation;

namespace OKR.API.StartUp
{
    public class ServiceRepoMapping
    {
        public ServiceRepoMapping() { }
        public void Mapping(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IRefreshTokenModelRepository, RefreshTokenModelRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITargetTypeRepository, TargetTypeRepository>();
            builder.Services.AddScoped<IObjectiveRepository, ObjectiveRepository>();
            builder.Services.AddScoped<IKeyResultRepository, KeyResultRepository>();


            builder.Services.AddScoped<IAuthencationService, AuthencationService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<ITargetTypeService, TargetTypeService>();
            builder.Services.AddScoped<IObjectiveService, ObjectiveService>();
        }
    }
}
