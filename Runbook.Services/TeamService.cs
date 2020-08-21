using Dapper;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Runbook.Services
{
    /// <summary>
    /// This service class is used for team operations
    /// </summary>
    public class TeamService : ITeamService
    {
        private readonly IDbConnection _Idbconnection;

        /// <summary>
        /// This constructor is to inject IDBConnection using constructor dependency injuction
        /// </summary>
        /// <param name="dbConnection"></param>
        public TeamService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

        /// <summary>
        /// This method creates the team the in team table
        /// </summary>
        /// <param name="team"></param>
        /// <returns>integer value</returns>
        public async Task<int> CreateTeam(Team team)
        {
            try
            {
                string getTeam = @"SELECT * FROM dbo.[Team] Where TeamName = @TeamName";
                string createTeamCmd = @"Insert into dbo.[Team](TeamName,Description,TenantId,Status) 
                                        Values(@TeamName,@Description,@TenantId,@Status)";
                int teamCreated;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    Team isTeamAvailable = con.QueryFirstOrDefault<Team>(getTeam, new {TeamName = team.TeamName});
                    
                    if(isTeamAvailable != null)
                    {
                        throw new Exception("Team with same name exist");
                    }

                    teamCreated = await con.ExecuteAsync(createTeamCmd, 
                        new Team{
                            TeamName = team.TeamName,
                            Description = team.Description,
                            TenantId = team.TenantId,
                            Status = true
                        });
                    con.Close();
                }
                return teamCreated;
            }
            catch(Exception ex){
                throw ex;
            }
        }

        /// <summary>
        /// This method returns the list of teams for a particular tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of teams</returns>
        public async Task<IEnumerable<Team>> GetAllTeams(int tenantId)
        {
            try
            {
                string getAllTeamsCmd = @"SELECT * FROM dbo.TEAM WHERE TenantId = @TenantId";
                IEnumerable<Team> teams = null;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    teams = await con.QueryAsync<Team>(getAllTeamsCmd, new {TenantId = tenantId});
                    con.Close();
                }
                return teams;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        /// <summary>
        /// It is used to get the team by Team id
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public async Task<Team> GetTeam(int teamId)
        {
            try
            {
                string getTeamCmd = @"SELECT * FROM dbo.TEAM WHERE TeamId = @TeamId";
                Team team = null;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    team = await con.QueryFirstOrDefaultAsync<Team>(getTeamCmd, new {TeamId = teamId});
                    con.Close();
                }
                return team;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> AddMembersToTeam(List<User> users,int teamId)
        {
            try
            {
                string addMembersToTeamCmd = @"Insert into dbo.[TeamUsers](UserId,TeamId)
                                                    Values(@UserId,@TeamId)";
                int membersAdded;
                List<TeamUser> membersToAdd = new List<TeamUser>();
                foreach (var user in users)
                {
                    membersToAdd.Add(new TeamUser(){
                        UserId = user.UserId,
                        TeamId = teamId
                    });
                }
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();                    
                    membersAdded = await con.ExecuteAsync(addMembersToTeamCmd, membersToAdd);
                    con.Close();
                }
                return membersAdded;
            }
            catch(Exception ex){
                throw ex;
            }
        }

        public async Task<IEnumerable<User>> GetTeamMembers(int teamId)
        {
            try
            {
                string getAllTeamsUsersCmd = @"SELECT U.UserId,FirstName,LastName,UserEmail,TenantId 
                                            FROM dbo.[User] U 
                                        JOIN dbo.[TeamUsers] TU ON U.UserId = TU.UserId
                                        WHERE TU.TeamId = @TeamId";
                IEnumerable<User> users = null;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    users = await con.QueryAsync<User>(getAllTeamsUsersCmd, new {TeamId = teamId});
                    con.Close();
                }
                return users;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveUserFromTeam(int teamId,int userId)
        {
            try
            {
                string removeTeamsUsersCmd = @"DELETE FROM dbo.[TeamUsers] WHERE UserId = @UserId AND TeamId = @TeamId";
                bool isUserDeleted = false;
                int userDeleted;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    userDeleted = await con.ExecuteAsync(removeTeamsUsersCmd, new {TeamId = teamId, UserId = userId});
                    con.Close();
                }
                if(userDeleted > 0)
                    return true;
                else
                    return false;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}