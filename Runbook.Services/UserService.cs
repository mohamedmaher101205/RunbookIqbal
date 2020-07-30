using Dapper;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Runbook.Services
{
    public class UserService : IUserService
    {
        private readonly IDbConnection _Idbconnection;

        public UserService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

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
    }
}