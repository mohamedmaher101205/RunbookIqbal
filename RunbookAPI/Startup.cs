using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using RunbookAPI.Models;
using RunbookAPI.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cors;
using RunbookAPI.Filters;

namespace RunbookAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(config => {
                config.Filters.Add<RefreshTokenAttribute>();
            });

            AppDomain.CurrentDomain.SetData("DataDirectory", "C:\\Projects\\Company\\RunBook\\RunBook-BE\\RunBook-API\\RunbookAPI");

            Console.WriteLine("DB Connection  => "+this.Configuration.GetConnectionString("Default"));

            services.AddTransient<IDbConnection>((connection) =>
                new SqlConnection(this.Configuration.GetConnectionString("Default")));

            services.AddScoped<IDataService,DataService>();

            services.AddScoped<IAuthService,AuthService>();

            services.AddScoped<IApplicationService,ApplicationService>();

            services.AddScoped<IUserService,UserService>();

            services.AddScoped<IJwtTokenGenerator,JwtTokenGenerator>();

            services.AddTransient<IMailService,MailService>();
            
            services.AddCors(c =>  
            {  
                c.AddPolicy("AllowAllHeaders", options => options.WithOrigins("http://localhost:3000","https://localhost:3000")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .WithExposedHeaders("Authorization"));  
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = Configuration["Jwt:Issuer"],
                   ValidAudience = Configuration["Jwt:Issuer"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                }
                );

            services.AddSwaggerGen(c =>
            {
               c.SwaggerDoc("v1", new OpenApiInfo { Title = "Runbook", Version = "v1" });
            });

            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("AllowAllHeaders");

            //app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Runbook V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
