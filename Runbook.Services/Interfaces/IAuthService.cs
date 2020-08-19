using Runbook.Models;
using System.Collections.Generic;

namespace Runbook.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        AuthRequest AuthenticateUser(User user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
       IEnumerable<InviteUsers> RegisterUser(User user, out string msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        AuthRequest OpenIdAuthenticateUser(User user);

    }
}