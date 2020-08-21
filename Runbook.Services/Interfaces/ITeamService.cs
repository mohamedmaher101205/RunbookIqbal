using System;
using System.Collections.Generic;
using System.Text;
using Runbook.Models;
using System.Threading.Tasks;

namespace Runbook.Services.Interfaces
{
    /// <summary>
    /// This interface is for Team service
    /// </summary>
    public interface ITeamService
    {
        /// <summary>
        /// This service creates the teams
        /// </summary>
        /// <param name="team"></param>
        /// <returns>int</returns>
        Task<int> CreateTeam(Team team);

        /// <summary>
        /// This service gets all the teams for a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of teams</returns>
        Task<IEnumerable<Team>> GetAllTeams(int tenantId);

        Task<Team> GetTeam(int teamId);

        Task<int> AddMembersToTeam(List<User> users,int teamId);

        Task<IEnumerable<User>> GetTeamMembers(int teamId);

        Task<bool> RemoveUserFromTeam(int teamId,int userId);
    }
}
