using Runbook.Models;
using System.Collections.Generic;

namespace Runbook.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        User GetUser(User user);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        Tenant GetTenant(int tenantId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        IEnumerable<User> GetAllUsers(int tenantId);

        bool CreateInviteUsers(InviteUsers inviteUsers);
    }
}