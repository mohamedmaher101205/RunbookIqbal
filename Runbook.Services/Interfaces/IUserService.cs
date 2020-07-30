using Runbook.Models;
using System.Collections.Generic;

namespace Runbook.Services.Interfaces
{
    public interface IUserService
    {
        User GetUser(User user);

        Tenant GetTenant(int tenantId);

        IEnumerable<User> GetAllUsers(int tenantId);
    }
}