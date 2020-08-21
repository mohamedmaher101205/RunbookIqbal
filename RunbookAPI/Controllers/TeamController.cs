using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Runbook.API.Controllers
{
    /// <summary>
    /// This TeamController class is responsible for team related CRUD operations 
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _team;
        private readonly ILogger _logger;

        /// <summary>
        /// This contructor is to inject object using dependency injection
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="team"></param>
        public TeamController(ILogger<TeamController> logger, ITeamService team)
        {
            _logger = logger;
            _team = team;
        }

        /// <summary>
        /// Create a team
        /// </summary>
        /// <param name="team"></param>
        /// <returns>Success message</returns>
        [HttpPost]
        [Route("CreateTeam")]
        public async Task<ActionResult> CreateTeam([FromBody] Team team)
        {
            try
            {
                if (!string.IsNullOrEmpty(team.TeamName) && team.TenantId > 0)
                {

                    int res = await _team.CreateTeam(team);

                    if (res <= 0)
                    {
                        _logger.LogError("Unsuccessfull while creating the Team in CreateTeam");
                        return NotFound("Unsuccessfull while creating the Team");
                    }

                    return Ok("Team created successfully");
                }
                else
                {
                    _logger.LogError($"Invalid Team Name : {team.TeamName} or TenantId : {team.TenantId} in CreateTeam");
                    return BadRequest($"Invalid Team Name : {team.TeamName} or TenantId : {team.TenantId}");
                }
            }
            catch (Exception ex)
            {
                if(ex.Message == "Team with same name exist")
                {
                    _logger.LogError($"Team {team.TeamName} already exist for the Tenant");
                    return Conflict($"Team {team.TeamName} already exist for the Tenant");
                }
                _logger.LogError($"Internal Server Error : {ex} in CreateTeam");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        /// <summary>
        /// Get all the teams for a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of teams</returns>
        [HttpGet]
        [Route("GetTeams/{tenantId}")]
        public async Task<ActionResult> GetAllTeams(int tenantId)
        {
            try
            {
                if (tenantId > 0)
                {
                    var teams = await _team.GetAllTeams(tenantId);

                    if(teams == null)
                    {
                        _logger.LogError($"No teams found for the tenantId : {tenantId} in GetAllTeams");
                        return NotFound($"No teams found for the tenantId : {tenantId}");
                    }

                    return Ok(teams);
                }
                else
                {
                    _logger.LogError($"Invalid TenantId : {tenantId} in GetAllTasks");
                    return BadRequest($"Invalid TenantId : {tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server Error in GetAllTeams : {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("GetTeam/{teamId}")]
        public async Task<ActionResult> GetTeam(int teamId)
        {
            try
            {
                if (teamId > 0)
                {
                    var team = await _team.GetTeam(teamId);

                    if(team == null)
                    {
                        _logger.LogError($"No team found for the teamId : {teamId} in GetTeam");
                        return NotFound($"No team found for the teamId : {teamId}");
                    }

                    return Ok(team);
                }
                else
                {
                    _logger.LogError($"Invalid teamId : {teamId} in GetTask");
                    return BadRequest($"Invalid teamId : {teamId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server Error in GetTeam : {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("AddMembersToTeam/{teamId}")]
        public async Task<ActionResult> AddMembersToTeam([FromBody] List<User> users,int teamId)
        {
            try
            {
                if (users.Count > 0 && teamId > 0)
                {
                    int res = await _team.AddMembersToTeam(users,teamId);

                    if (res <= 0)
                    {
                        _logger.LogError("Unsuccessfull while adding members to team in AddMembersToTeam");
                        return NotFound("Unsuccessfull while adding members to team ");
                    }

                    return Ok("Members Added successfully");
                }
                else
                {
                    _logger.LogError($"Invalid Users : {users} or TeamId : {teamId} in AddMembersToTeam");
                    return BadRequest($"Invalid Users : {users} or TeamId : {teamId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error : {ex} in AddMembersToTeam");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("GetTeamUsers/{teamId}")]
        public async Task<ActionResult> GetTeamUsers(int teamId)
        {
            try
            {
                if (teamId > 0)
                {
                    var users = await _team.GetTeamMembers(teamId);

                    if(users == null)
                    {
                        _logger.LogError($"No team Users found for the teamId : {teamId} in GetTeamUsers");
                        return NotFound($"No team users found for the teamId : {teamId}");
                    }

                    return Ok(users);
                }
                else
                {
                    _logger.LogError($"Invalid teamId : {teamId} in GetTeamUsers");
                    return BadRequest($"Invalid teamId : {teamId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server Error in GetTeamUsers : {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete]
        [Route("RemoveTeamUser/{teamId}/{userId}")]
        public async Task<ActionResult> RemoveTeamUsers(int teamId,int userId)
        {
            try
            {
                if (teamId > 0 && userId > 0)
                {
                    var isUserDeleted = await _team.RemoveUserFromTeam(teamId,userId);

                    if(!isUserDeleted)
                    {
                        _logger.LogError($"No User found for the teamId : {teamId} to remove in RemoveTeamUsers");
                        return NotFound($"No User found for the teamId : {teamId} to remove");
                    }

                    return Ok("User removed");
                }
                else
                {
                    _logger.LogError($"Invalid teamId : {teamId} or UserId : {userId} in RemoveTeamUsers");
                    return BadRequest($"Invalid teamId : {teamId} or UserId : {userId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server Error in RemoveTeamUsers : {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}