using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Runbook.API.Controllers
{

    /// <summary>
    /// This EnvironmentController class have methods to performing create an environment and 
    /// select all environments
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EnvironmentController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEnvironmentService _env;

        /// <summary>
        /// This contructor is to inject object using dependency injection
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="env"></param>
        public EnvironmentController(ILogger<EnvironmentController> logger, IEnvironmentService env)
        {
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Create an environment
        /// </summary>
        /// <param name="env"></param>
        /// <param name="tenantId"></param>
        /// <returns>Success message</returns>
        [HttpPost]
        [Route("CreateEnvironment/{tenantId}")]
        public IActionResult CreateCustomEnvironment([FromBody] Environments env, int tenantId)
        {
            try
            {
                if (tenantId > 0)
                {
                    int res = _env.CreateEnvironment(env, tenantId);
                    if (res > 0)
                    {
                        return Ok($"{res} Environments inserted");
                    }
                    return Ok($"Environments failed to insert");
                }
                else
                {
                    _logger.LogError($"Invalid TenantId in CreateCustomEnvironment : {tenantId}");
                    return BadRequest($"Invalid TenantId : {tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server Error in CreateCustomEnvironment : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Get all environment list
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of environment</returns>
        [HttpGet]
        [Route("GetEnvironments/{tenantId}")]
        public ActionResult<IEnumerable<Environments>> GetAllEnvironments(int tenantId)
        {
            try
            {
                return Ok(_env.GetAllEnvironments(tenantId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in GetAllEnvironments : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}