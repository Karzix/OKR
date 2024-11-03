using OKR.Models.Context;
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
            builder.Services.AddScoped<IObjectivesRepository, ObjectivesRepository>();
            builder.Services.AddScoped<IKeyResultRepository, KeyResultRepository>();
            builder.Services.AddScoped<IProgressUpdatesRepository, ProgressUpdatesRepository>();
            builder.Services.AddScoped<IEvaluateTargetRepository, EvaluateTargetRepository>();
            builder.Services.AddScoped<IDepartmentProgressApprovalRepository, DepartmentProgressApprovalRepository>();

            builder.Services.AddScoped<IAuthencationService, AuthencationService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IObjectivesService, ObjectiveService>();
            builder.Services.AddScoped<IKeyResultsService, KeyResultsService>();
            builder.Services.AddScoped<IProgressUpdatesService, ProgressUpdatesService>();
            builder.Services.AddScoped<IEvaluateTargetService, EvaluateTargetService>();
            builder.Services.AddScoped<IEntityObjectivesService, EntityObjectivesService>();
            builder.Services.AddScoped<IDepartmentProgressApprovalService, DepartmentProgressApprovalService>();

            builder.Services.AddScoped<SendReminderEmailJob>();
        }
    }
}
