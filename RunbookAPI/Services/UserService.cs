using System.Collections;
using System;
using RunbookAPI.Models;
using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace RunbookAPI.Services
{
    public class UserService : IUserService
    {
        private IDbConnection _Idbconnection;

        public UserService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

        public int LinkUsers(int tenantId,int[] userIds)
        {
            try{
                string linkusercmd = @"INSERT INTO [Runbook].[dbo].[TenantUsers](TenantId,UserId) 
                                        VALUES(@TenantId,@UserId)";

                List<TenantUser> tenantUsers = new List<TenantUser>();
                int insertedRows = 0;
                foreach (var userid in userIds)
                {
                    tenantUsers.Add(new TenantUser
                    {
                        TenantId = tenantId,
                        UserId = userid
                    });
                }

                using(IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    insertedRows = con.Execute(linkusercmd,tenantUsers);
                    con.Close();
                }

                return insertedRows;
            }
            catch(Exception ex){
                throw ex;
            }
        }

        public IEnumerable<User> GetLinkedUsers(int tenantId)
        {
            try{
                string getUsersCmd = @"SELECT UserId,FirstName,LastName,UserEmail FROM [Runbook].[dbo].[User] 
                                        Where UserId in (
                                            SELECT UserId FROM [Runbook].[dbo].[TenantUsers] WHERE TenantId = @TenantId
                                        )";
                
                IEnumerable<User> linkedUsers = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    linkedUsers = con.Query<User>(getUsersCmd,new{TenantId = tenantId});
                    con.Close();
                }
                return linkedUsers;
            }
            catch(Exception ex){
                throw ex;
            }
        }

        public int CreateGroup(int tenantId, Group group)
        {
            try{
                string createGroupCmd = @"INSERT INTO [Runbook].[dbo].[Group](TenantId,GroupName,Description) 
                                            VALUES(@TenantId,@GroupName,@Description)";
                int groupCreated = 0;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    var sqltrans = con.BeginTransaction();
                    groupCreated = con.Execute(createGroupCmd,
                        new{
                            TenantId =tenantId,
                            GroupName = group.GroupName,
                            Description = group.Description
                        },sqltrans);
                    if(groupCreated > 0){
                        sqltrans.Commit();
                    }
                    else{
                        sqltrans.Rollback();
                    }
                    con.Close();
                }
                return groupCreated;
            }
            catch(Exception ex){
                throw ex;
            }
        }

        public IEnumerable<Group> GetTenantGroups(int tenantId)
        {
            try{
                string groupscmd = @"SELECT * FROM [Runbook].[dbo].[Group] WHERE TenantId = @TenantId";

                IEnumerable<Group> groups = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    groups = con.Query<Group>(groupscmd,new {TenantId = tenantId});
                    con.Close();
                }
                return groups;
            }
            catch(Exception ex){
                throw ex;
            }
        }

        public int AddUsersToGroup(int groupId,int[] userIds)
        {
            try{
                string groupusercmd = @"INSERT INTO [Runbook].[dbo].[GroupUsers](GroupId,UserId) 
                                        VALUES(@GroupId,@UserId)";

                List<GroupUser> tenantUsers = new List<GroupUser>();
                int insertedRows = 0;
                foreach (var userid in userIds)
                {
                    tenantUsers.Add(new GroupUser
                    {
                        GroupId = groupId,
                        UserId = userid
                    });
                }

                using(IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    insertedRows = con.Execute(groupusercmd,tenantUsers);
                    con.Close();
                }

                return insertedRows;
            }
            catch(Exception ex){
                throw ex;
            }
        }

        public IEnumerable<User> GetGroupUsers(int groupId)
        {
            try{
                string getUsersCmd = @"SELECT UserId,FirstName,LastName,UserEmail FROM [Runbook].[dbo].[User] 
                                        Where UserId in (
                                            SELECT UserId FROM [Runbook].[dbo].[GroupUsers] WHERE GroupId = @GroupId
                                        )";
                
                IEnumerable<User> groupUsers = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    groupUsers = con.Query<User>(getUsersCmd,new{GroupId = groupId});
                    con.Close();
                }
                return groupUsers;
            }
            catch(Exception ex){
                throw ex;
            }
        }

        public Tenant GetTenant(int tenantId)
        {
            try{
                string getTenantCmd = @"SELECT TenantId,TenantName,Domain FROM [Runbook].[dbo].[Tenant]
                                        WHERE TenantId = @TenantId";

                Tenant tenant = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    tenant = con.QueryFirstOrDefault<Tenant>(getTenantCmd,new {TenantId = tenantId});
                    con.Close();
                }
                return tenant;
            }
            catch(Exception ex){
                throw ex;
            }
        }

        public int CreateEnvironment(Environments env,int tenantId)
        {
            try{
                string userDefinedEnv = @"INSERT INTO [Runbook].[dbo].[UserDefinedEnvironments](Environment,TenantId)
                                            VALUES(@Environment,@TenantId)";
                int insertedEnv = 0;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    var sqlTrans = con.BeginTransaction();
                    insertedEnv = con.Execute(userDefinedEnv,new 
                        {
                            Environment = env.Environment,
                            TenantId = tenantId
                        },sqlTrans);
                    if(insertedEnv > 0 ){
                        sqlTrans.Commit();
                    }
                    else{
                        sqlTrans.Rollback();
                    }
                    con.Close();
                }
                return insertedEnv;
            }catch(Exception ex){
                throw ex;
            }
        }
    }
}