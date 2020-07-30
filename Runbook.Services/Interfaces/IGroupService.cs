using System;
using System.Collections.Generic;
using System.Text;
using Runbook.Models;

namespace Runbook.Services.Interfaces
{
    public interface IGroupService
    {
        int CreateGroup(int tenantId, Group group);

        IEnumerable<Group> GetTenantGroups(int tenantId);

        int AddUsersToGroup(int groupId, int[] userIds);

        IEnumerable<User> GetGroupUsers(int groupId);

        IEnumerable<Permissions> GetPermissions();
    }
}
