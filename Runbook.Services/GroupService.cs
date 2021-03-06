﻿using Dapper;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Runbook.Services
{
    /// <summary>
    /// This GroupService class have methods to performing create a group and get users group,
    /// select all group,select all permission, add user to the group, get tenant group
    /// </summary>
    public class GroupService : IGroupService
    {
        private readonly IDbConnection _Idbconnection;

        /// <summary>
        /// This constructor is to inject IDBConnection using constructor dependency injuction
        /// </summary>
        /// <param name="dbConnection"></param>
        public GroupService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

        /// <summary>
        ///  create a group
        /// </summary>
        /// <param name="group"></param>
        /// <param name="tenantId"></param>
        /// <returns>Success message</returns>
        public int CreateGroup(int tenantId, Group group)
        {
            try
            {
                int createdgroup = 0;
                string groupPermissionCmd = @"INSERT INTO [dbo].[GroupPermissions](GroupId,PermissionId) VALUES(@GroupId,@PermissionId)";
                var groupparams = new DynamicParameters();
                groupparams.Add("@GroupName", group.GroupName);
                groupparams.Add("@Description", group.Description);
                groupparams.Add("@TenantId", tenantId);
                groupparams.Add("@InsertedGroupId", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                List<GroupPermissions> groupPermission = new List<GroupPermissions>();

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    createdgroup = con.Execute("[dbo].sp_CreateGroup", groupparams, commandType: CommandType.StoredProcedure);
                    int insertedGroupId = groupparams.Get<int>("@InsertedGroupId");

                    foreach (var permissionId in group.PermissionIds)
                    {
                        groupPermission.Add(new GroupPermissions
                        {
                            GroupId = insertedGroupId,
                            PermissionId = permissionId
                        });
                    }

                    var groupPermissionInserted = con.Execute(groupPermissionCmd, groupPermission);
                    con.Close();
                }
                return createdgroup;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  Read group list based on tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>Tenant Group list</returns>
        public IEnumerable<Group> GetTenantGroups(int tenantId)
        {
            try
            {
                string groupscmd = @"SELECT * FROM [dbo].[Group] WHERE TenantId = @TenantId";

                IEnumerable<Group> groups = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    groups = con.Query<Group>(groupscmd, new { TenantId = tenantId });
                    con.Close();
                }
                return groups;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Read list of permissions
        /// </summary>
        /// <returns>List of permission</returns>
        public IEnumerable<Permissions> GetPermissions()
        {
            try
            {
                string permissionCmd = "SELECT * FROM [dbo].[Permissions]";
                IEnumerable<Permissions> permissions = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    permissions = con.Query<Permissions>(permissionCmd);
                    con.Close();
                }
                return permissions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// add users to the group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userIds"></param>
        /// <returns>receive added users message</returns>
        public int AddUsersToGroup(int groupId, int[] userIds)
        {
            try
            {
                string groupusercmd = @"INSERT INTO [dbo].[GroupUsers](GroupId,UserId) 
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

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    insertedRows = con.Execute(groupusercmd, tenantUsers);
                    con.Close();
                }

                return insertedRows;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Reading list of users based on group id
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>list of users</returns>
        public IEnumerable<User> GetGroupUsers(int groupId)
        {
            try
            {
                string getUsersCmd = @"SELECT UserId,FirstName,LastName,UserEmail FROM [dbo].[User] 
                                        Where UserId in (
                                            SELECT UserId FROM [dbo].[GroupUsers] WHERE GroupId = @GroupId
                                        )";

                IEnumerable<User> groupUsers = null;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    groupUsers = con.Query<User>(getUsersCmd, new { GroupId = groupId });
                    con.Close();
                }
                return groupUsers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
