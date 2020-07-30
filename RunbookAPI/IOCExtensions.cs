using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Runbook.Services;
using Runbook.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace Runbook.API
{
    public static class IOCExtensions
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDbConnection>((connection) =>
                new SqlConnection(configuration.GetConnectionString("Default"))
            );

            services.AddScoped<IBookService, BookService>();

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IApplicationService, ApplicationService>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<ITaskService, TaskService>();

            services.AddScoped<IStageService, StageService>();

            services.AddScoped<IResourceService, ResourceService>();

            services.AddScoped<IEnvironmentService, EnvironmentService>();

            services.AddScoped<IGroupService, GroupService>();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddTransient<IMailService, MailService>();

            return services;
        } 
    }
}
