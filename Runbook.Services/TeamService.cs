using Dapper;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    }
}