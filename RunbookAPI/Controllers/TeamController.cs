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
    }
}