using System;
using System.Collections.Generic;
using System.Text;
using Runbook.Models;

namespace Runbook.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGroupService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        int CreateGroup(int tenantId, Group group);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        IEnumerable<Group> GetTenantGroups(int tenantId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        int AddUsersToGroup(int groupId, int[] userIds);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        IEnumerable<User> GetGroupUsers(int groupId);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Permissions> GetPermissions();
    }
}
