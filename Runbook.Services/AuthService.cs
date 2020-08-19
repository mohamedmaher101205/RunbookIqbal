using Dapper;
using Microsoft.Extensions.Configuration;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Runbook.Services
{
    /// <summary>
    /// This ApplicationService class used to register the user and authenticate and authorize 
    /// the user and generate the token
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserService _user;
        private readonly IConfiguration _config;
        private readonly IDbConnection _Idbconnection;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        /// <summary>
        /// This constructor is to inject IDBConnection,user service,configuration,jwt token generator
        /// object using constructor dependency injuction
        /// </summary>
        /// <param name="user"></param>
        /// <param name="config"></param>
        /// <param name="dbConnection"></param>
        /// <param name="jwtTokenGenerator"></param>
        public AuthService(IUserService user, IConfiguration config, IDbConnection dbConnection, IJwtTokenGenerator jwtTokenGenerator)
        {
            _user = user;
            _config = config;
            _Idbconnection = dbConnection;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        /// <summary>
        /// register the user using sign up
        /// </summary>
        /// <param name="user"></param>
        /// <returns>success or failed or user exist message</returns>
        public IEnumerable<InviteUsers> RegisterUser(User user, out string msg)
        {
            try
            {
                IEnumerable<InviteUsers> userInvite = null;

                if (!string.IsNullOrEmpty(user.Password))
                {
                    user.Password = EncodePasswordToBase64(user.Password);

                }
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
                            UserParams.Add("@IsAdmin", "true");
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
                        var inviteUserparams = new DynamicParameters();
                        inviteUserparams.Add("@InviteUserEmailId", user.UserEmail);
                        userInvite = con.Query<InviteUsers>("[dbo].sp_getInviteUserDetails", inviteUserparams, commandType: CommandType.StoredProcedure);
                    }

                    else
                    {
                        msg = "User exist";

                        return userInvite;
                    }
                }
                if (userRegistered > 0)
                {
                    msg = "successfull";
                    return userInvite;
                }
                else
                {
                    msg = "failed";
                    return userInvite;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Authenticate user using username and password and create the token
        /// </summary>
        /// <param name="user"></param>
        /// <returns>generated token</returns>
        public AuthRequest AuthenticateUser(User user)
        {
            try
            {
                User response = null;
                AuthRequest token = null;

                response = _user.GetUser(user);

                if (!string.IsNullOrEmpty(response.Password))
                {
                    response.Password = DecodeFrom64(response.Password);

                }
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

        /// <summary>
        /// check user's email exist and generate token
        /// </summary>
        /// <param name="user"></param>
        /// <returns>token</returns>
        public AuthRequest OpenIdAuthenticateUser(User user)
        {
            try
            {
                User response = null;
                AuthRequest token = null;

                response = _user.GetUser(user);
                if (!string.IsNullOrEmpty(response.Password))
                {
                    response.Password = DecodeFrom64(response.Password);

                }

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

        /// <summary>
        ///  copy default for application type, environments, resource types
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="con"></param>
        /// <returns></returns>
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
        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        public static string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
    }
}
