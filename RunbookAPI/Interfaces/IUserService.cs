using System.Collections;
using System.Collections.Generic;

namespace RunbookAPI.Models
{
    public interface IUserService
    {
        int LinkUsers(int tenantId,int[] userIds);

        IEnumerable<User> GetLinkedUsers(int tenantId);

        int CreateGroup(int tenantId, Group group);

        IEnumerable<Group> GetTenantGroups(int tenantId);

        int AddUsersToGroup(int groupId,int[] userIds);

        IEnumerable<User> GetGroupUsers(int groupId);

        Tenant GetTenant(int tenantId);

        int CreateEnvironment(Environments env,int tenantId);
    }
}