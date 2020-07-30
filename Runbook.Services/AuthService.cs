using Dapper;
using Microsoft.Extensions.Configuration;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Runbook.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _user;
        private readonly IConfiguration _config;
        private readonly IDbConnection _Idbconnection;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IUserService user, IConfiguration config, IDbConnection dbConnection, IJwtTokenGenerator jwtTokenGenerator)
        {
            _user = user;
            _config = config;
            _Idbconnection = dbConnection;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public string RegisterUser(User user)
        {
            try
            {
                string[] email = user.UserEmail.Split("@");
                string domain = email[1];
                string tenantcmd = @"SELECT TenantId FROM [dbo].[Tenant] WHERE Domain = @Domain";
                string userCmd = @"SELECT UserId FROM [dbo].[User] WHERE UserEmail = @UserEmail";

                User userExist = null;
                int userRegistered = 0;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    userExist = con.QuerySingleOrDefault<User>(userCmd, new { UserEmail = user.UserEmail });
                    if (userExist == null)
                    {
                        var res = con.QuerySingleOrDefault(tenantcmd, new { Domain = domain });
                        var UserParams = new DynamicParameters();
                        UserParams.Add("@FirstName", user.FirstName);
                        UserParams.Add("@LastName", user.LastName);
                        UserParams.Add("@UserEmail", user.UserEmail);
                        UserParams.Add("@Password", user.Password);
                        UserParams.Add("@RegisteredUserId", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                        if (res != null)
                        {
                            UserParams.Add("@TenantId", res.TenantId);
                            UserParams.Add("@IsAdmin", "false");
                            userRegistered = con.Execute("[dbo].sp_CreateUser", UserParams, commandType: CommandType.StoredProcedure);
                            int createdUserId = UserParams.Get<int>("@RegisteredUserId");
                        }
                        else
                        {
                            string[] tenantNameArr = domain.Split(".");
                            string tenantName = tenantNameArr[0];
                            var TenantParams = new DynamicParameters();
                            TenantParams.Add("@CreatedTenantId", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                            TenantParams.Add("@TenantName", char.ToUpper(tenantName[0]) + tenantName.Substring(1));
                            TenantParams.Add("@DomainName", domain);
                            var createTenant = con.Execute("[dbo].sp_CreateTenant", TenantParams, commandType: CommandType.StoredProcedure);
                            int createdTenantId = TenantParams.Get<int>("@CreatedTenantId");

                            CopyDefaluts(createdTenantId, con);

                            UserParams.Add("@TenantId", createdTenantId);
                            UserParams.Add("@IsAdmin", "true");
                            userRegistered = con.Execute("[dbo].sp_CreateUser", UserParams, commandType: CommandType.StoredProcedure);
                            int createdUserId = UserParams.Get<int>("@RegisteredUserId");
                        }
                    }
                    else
                    {
                        return "User with same email already exist";
                    }
                }
                if (userRegistered > 0)
                {
                    return "User registered successfully";
                }
                else
                {
                    return "User registration failed";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AuthRequest AuthenticateUser(User user)
        {
            try
            {
                User response = null;
                AuthRequest token = null;

                response = _user.GetUser(user);
                if (response != null && response.Password.Equals(user.Password))
                {
                    //JwtTokenGenerator jwt = new JwtTokenGenerator(_config);
                    token = _jwtTokenGenerator.GenerateToken(response);
                }
                return token;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AuthRequest OpenIdAuthenticateUser(User user)
        {
            try
            {
                User response = null;
                AuthRequest token = null;

                response = _user.GetUser(user);

                if (response != null && user.UserEmail.Equals(response.UserEmail))
                {
                    // JwtTokenGenerator jwt = new JwtTokenGenerator(_config);
                    token = _jwtTokenGenerator.GenerateToken(response);
                }

                return token;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CopyDefaluts(int tenantId, IDbConnection con)
        {
            try
            {
                string defalutEnvs = @"SELECT Environment FROM [dbo].[Environments]";
                string defaultAppTypes = @"SELECT AppTypeName FROM [dbo].[ApplicationType]";
                string defalutResourceTypes = @"SELECT ResourceTypeName FROM [dbo].[ResourceTypes]";

                string tenantEnvsCmd = @"INSERT INTO [dbo].[userdefinedenvironments](Environment,TenantId) 
	                                        VALUES(@Environment,@TenantId)";

                string tenantAppTypeCmd = @"INSERT INTO [dbo].[UserDefinedapplicationType](AppTypeName,TenantId)
                                            VALUES(@AppTypeName,@TenantId)";

                string tenantResourceTypeCmd = @"INSERT INTO [dbo].[UserDefinedResourceTypes](ResourceTypeName,TenantId)
	                                            VALUES (@ResourceTypeName,@TenantId)";

                List<Environments> tenantEnvs = new List<Environments>();
                List<ApplicationType> tenantAppTypes = new List<ApplicationType>();
                List<ResourceType> tenantResourceTypes = new List<ResourceType>();

                IEnumerable<Environments> systemEnvs = con.Query<Environments>(defalutEnvs);
                IEnumerable<ApplicationType> systemAppTypes = con.Query<ApplicationType>(defaultAppTypes);
                IEnumerable<ResourceType> systemResourceTypes = con.Query<ResourceType>(defalutResourceTypes);

                foreach (var env in systemEnvs)
                {
                    tenantEnvs.Add(new Environments
                    {
                        Environment = env.Environment,
                        TenantId = tenantId
                    });
                }

                foreach (var apptype in systemAppTypes)
                {
                    tenantAppTypes.Add(new ApplicationType
                    {
                        AppTypeName = apptype.AppTypeName,
                        TenantId = tenantId
                    });
                }

                foreach (var restype in systemResourceTypes)
                {
                    tenantResourceTypes.Add(new ResourceType
                    {
                        ResourceTypeName = restype.ResourceTypeName,
                        TenantId = tenantId
                    });
                }

                var envres = con.Execute(tenantEnvsCmd, tenantEnvs);
                var apptyperes = con.Execute(tenantAppTypeCmd, tenantAppTypes);
                var restyperes = con.Execute(tenantResourceTypeCmd, tenantResourceTypes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}