using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Runbook.Services;
using Runbook.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace Runbook.API
{
    /// <summary>
    /// This class is to inject the object using IOC container
    /// </summary>
    public static class IOCExtensions
    {
        /// <summary>
        /// This method is to do dependency injection 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns> 
        public static IServiceCollection ResolveDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            string Conn = configuration.GetConnectionString("Default");
            Conn = AuthService.DecodeFrom64(Conn);

            services.AddTransient<IDbConnection>((connection) =>
                new SqlConnection(Conn)
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
