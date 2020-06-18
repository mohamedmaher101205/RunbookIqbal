using System.Collections;
using System;
using System.Text;
using System.Security.Claims;
using RunbookAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace RunbookAPI.Services
{
    public class AuthService : IAuthService
    {
        private IDataService _data;
        private IConfiguration _config;
        private IDbConnection _Idbconnection;
        private IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IDataService data,IConfiguration config,IDbConnection dbConnection,IJwtTokenGenerator jwtTokenGenerator)
        {
            _data = data;
            _config = config;
            _Idbconnection = dbConnection;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public bool RegisterUser(User user)
        {
            try{
                string[] email = user.UserEmail.Split("@");
                string domain = email[1];
                string tenantcmd = @"SELECT TenantId FROM [Runbook].[dbo].[Tenant] WHERE Domain = @Domain";

                var UserParams = new DynamicParameters();
                UserParams.Add("@FirstName",user.FirstName);
                UserParams.Add("@LastName",user.LastName);
                UserParams.Add("@UserEmail",user.UserEmail);
                UserParams.Add("@Password",user.Password);
                UserParams.Add("@RegisteredUserId", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                
                int userRegistered = 0;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    //var sqltrans = con.BeginTransaction();
                    var res = con.QuerySingleOrDefault(tenantcmd,new{Domain = domain});

                    if(res != null){
                        UserParams.Add("@TenantId",res.TenantId);
                        userRegistered = con.Execute("[Runbook].[dbo].sp_CreateUser", UserParams, commandType: CommandType.StoredProcedure);
                        int createdUserId = UserParams.Get<int>("@RegisteredUserId");
                    }
                    else{
                        string[] tenantNameArr = domain.Split(".");
                        string tenantName = tenantNameArr[0];
                        var TenantParams = new DynamicParameters();
                        TenantParams.Add("@CreatedTenantId", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                        TenantParams.Add("@TenantName",char.ToUpper(tenantName[0])+tenantName.Substring(1));
                        TenantParams.Add("@DomainName",domain);
                        var createTenant = con.Execute("[Runbook].[dbo].sp_CreateTenant", TenantParams, commandType: CommandType.StoredProcedure);
                        int createdTenantId = TenantParams.Get<int>("@CreatedTenantId");

                        CopyDefaluts(createdTenantId,con);
                        
                        UserParams.Add("@TenantId",createdTenantId);
                        userRegistered = con.Execute("[Runbook].[dbo].sp_CreateUser", UserParams, commandType: CommandType.StoredProcedure);
                        int createdUserId = UserParams.Get<int>("@RegisteredUserId");
                    }
                }
                if(userRegistered > 0){
                    return true;
                }
                else{
                    return false;
                }
            }catch(Exception ex){
                throw ex;
            }
        }

        public AuthRequest AuthenticateUser(User user)
        {
            try{
                User response = null;
                AuthRequest token = null;

                response = _data.GetUser(user);
                if(response != null && response.Password.Equals(user.Password))
                {
                    //JwtTokenGenerator jwt = new JwtTokenGenerator(_config);
                    token = _jwtTokenGenerator.GenerateToken(response);
                    return token;
                }
                return token;
            }catch(Exception ex){
                throw ex;
            }
        }

        public AuthRequest OpenIdAuthenticateUser(User user)
        {
            try{
                User response = null;
                AuthRequest token = null;

                response = _data.GetUser(user);

                if(response != null && user.UserEmail.Equals(response.UserEmail)){
                    JwtTokenGenerator jwt = new JwtTokenGenerator(_config);
                    token = jwt.GenerateToken(response);
                    return token;
                }

                return token;

            }catch(Exception ex){
                throw ex;
            }
        }

        private void CopyDefaluts(int tenantId,IDbConnection con){
            try{
                string defalutEnvs = @"SELECT Environment FROM [Runbook].[dbo].[Environments]";
                string defaultAppTypes = @"SELECT AppTypeName FROM [Runbook].[dbo].[ApplicationType]";
                string defalutResourceTypes = @"SELECT ResourceTypeName FROM [Runbook].[dbo].[ResourceTypes]";

                string tenantEnvsCmd = @"INSERT INTO [Runbook].[dbo].[userdefinedenvironments](Environment,TenantId) 
	                                        VALUES(@Environment,@TenantId)";

                string tenantAppTypeCmd = @"INSERT INTO [Runbook].[dbo].[UserDefinedapplicationType](AppTypeName,TenantId)
                                            VALUES(@AppTypeName,@TenantId)";

                string tenantResourceTypeCmd = @"INSERT INTO [Runbook].[dbo].[UserDefinedResourceTypes](ResourceTypeName,TenantId)
	                                            VALUES (@ResourceTypeName,@TenantId)";

                List<Environments> tenantEnvs = new List<Environments>();
                List<ApplicationType> tenantAppTypes = new List<ApplicationType>();
                List<ResourceType> tenantResourceTypes = new List<ResourceType>();

                    IEnumerable<Environments> systemEnvs = con.Query<Environments>(defalutEnvs);
                    IEnumerable<ApplicationType> systemAppTypes = con.Query<ApplicationType>(defaultAppTypes);
                    IEnumerable<ResourceType> systemResourceTypes = con.Query<ResourceType>(defalutResourceTypes);

                    foreach(var env in systemEnvs)
                    {
                        tenantEnvs.Add(new Environments{
                            Environment = env.Environment,
                            TenantId = tenantId
                        });
                    }

                    foreach(var apptype in systemAppTypes)
                    {
                        tenantAppTypes.Add(new ApplicationType{
                            AppTypeName = apptype.AppTypeName,
                            TenantId = tenantId
                        });
                    }

                    foreach (var restype in systemResourceTypes)
                    {
                        tenantResourceTypes.Add(new ResourceType{
                            ResourceTypeName = restype.ResourceTypeName,
                            TenantId = tenantId
                        });
                    }

                    var envres = con.Execute(tenantEnvsCmd,tenantEnvs);
                    var apptyperes = con.Execute(tenantAppTypeCmd,tenantAppTypes);
                    var restyperes = con.Execute(tenantResourceTypeCmd,tenantResourceTypes);
            }catch(Exception ex){
                throw ex;
            }
        }

    }
}