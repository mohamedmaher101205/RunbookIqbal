using Dapper;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Runbook.Services
{
    /// <summary>
    /// This UserService class have methods to performing to select particular tenant, 
    /// get all users,send email to the user
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IDbConnection _Idbconnection;

        /// <summary>
        /// This constructor is to inject IDBConnection using constructor dependency injuction
        /// </summary>
        /// <param name="dbConnection"></param>
        public UserService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User GetUser(User user)
        {
            try
            {
                User userResult = null;
                IEnumerable<string> permission = null;

                string sqlcmd = "SELECT * FROM [dbo].[User] where USEREMAIL=@useremail";
                string permissionCmd = @"SELECT Permission FROM [dbo].[Permissions] P
                                JOIN [dbo].[GroupPermissions] GP ON GP.PermissionId = P.PermissionId
                                JOIN [dbo].[Group] G ON G.GroupId = GP.GroupId
                                JOIN [dbo].[GroupUsers] GU ON GU.GroupId = G.GroupId
                            WHERE GU.UserId = @UserId";
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    userResult = con.QueryFirstOrDefault<User>(sqlcmd, new { useremail = user.UserEmail });
                    permission = con.Query<string>(permissionCmd, new { UserId = userResult.UserId });
                    userResult.Permissions = new List<string>(permission);
                    con.Close();
                }
                return userResult;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public Tenant GetTenant(int tenantId)
        {
            try
            {
                string getTenantCmd = @"SELECT TenantId,TenantName,Domain FROM [dbo].[Tenant]
                                        WHERE TenantId = @TenantId";

                Tenant tenant = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    tenant = con.QueryFirstOrDefault<Tenant>(getTenantCmd, new { TenantId = tenantId });
                    con.Close();
                }
                return tenant;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public IEnumerable<User> GetAllUsers(int tenantId)
        {
            try
            {
                string userscmd = @"SELECT UserId,FirstName,LastName,UserEmail FROM 
                                    [dbo].[User] Where TenantId = @TenantId";

                IEnumerable<User> users = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    users = con.Query<User>(userscmd, new { TenantId = tenantId });
                    con.Close();
                }

                return users;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        /// <summary>
        /// Fetch the InvitedUser List
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public List<InviteUsers> GetInviteUsers(int tenantId)
        {
            try
            {
                string userscmd = @"SELECT [Id]
                                          ,[InviteUserEmailId]
                                          ,[InviteUrl]
                                          ,[InviteRoleLevel]
                                          ,[Accepted]
                                          ,[TenantId]
                                    FROM [dbo].[InviteUser] Where TenantId = @TenantId and Accepted =1";

                IEnumerable<InviteUsers> users = null;
                IDbConnection con = _Idbconnection;
                con.Open();
                users = con.Query<InviteUsers>(userscmd, new
                {
                    TenantId = tenantId
                });
                con.Close();


                return users.AsList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool CreateInviteUsers(InviteUsers inviteUsers)
        {
            InviteUsers userExist = null;
            var UserEmail = inviteUsers.InviteUserEmailId;
            string userCmd = @"SELECT id FROM [dbo].[InviteUser] WHERE InviteUserEmailId = @InviteUserEmailId";
            var inviteUserparams = new DynamicParameters();
            inviteUserparams.Add("@InviteUserEmailId", inviteUsers.InviteUserEmailId);
            inviteUserparams.Add("@InviteUrl", inviteUsers.InviteUrl);
            inviteUserparams.Add("@InviteRoleLevel", inviteUsers.InviteRoleLevel);

            inviteUserparams.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();


                userExist = con.QuerySingleOrDefault<InviteUsers>(userCmd, new { InviteUserEmailId = UserEmail });
                if (userExist != null)
                {
                    return false;
                }
                var sqltrans = con.BeginTransaction();
                var createdInviteUser = con.Execute("[dbo].sp_CreateInviteUser", inviteUserparams, sqltrans, 0, commandType: CommandType.StoredProcedure);


                if (createdInviteUser > 0)
                {
                    sqltrans.Commit();
                }
                else
                {
                    sqltrans.Rollback();
                }
                con.Close();

                if (createdInviteUser > 0)
                    return true;

            }
            return false;
        }

    }
}