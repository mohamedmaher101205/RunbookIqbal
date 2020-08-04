using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Runbook.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _app;
        private readonly ILogger _logger;

        public ApplicationController(ILogger<ApplicationController> logger, IApplicationService app)
        {
            _logger = logger;
            _app = app;
        }

        [HttpPost]
        [Route("CreateApplication/{tenantId}")]
        public IActionResult CreateApplication([FromBody] Application app, int tenantId)
        {
            try
            {
                if (!string.IsNullOrEmpty(app.ApplicationName))
                {                   
                    var res = _app.CreateApplication(app, tenantId);
                    if (res)
                    {
                        return Ok("Application created successfully");
                    }
                    else
                    {
                        return BadRequest("Application Not created");
                    }
                }
                else
                {
                    _logger.LogError($"Empty Application name : {app} in CreateApplication");
                    return BadRequest($"Empty Application name : {app}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in CreateApplication : {ex} ");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("GetApplications/{tenantId}")]
        public ActionResult<IEnumerable<Application>> GetAllApplications(int tenantId)
        {
            try
            {
                if (tenantId > 0)
                {
                    return Ok(_app.GetAllApplications(tenantId));
                }
                else
                {
                    _logger.LogError($"Invalid TenantId in GetAllApplications : {tenantId}");
                    return BadRequest($"Invalid TenantId : {tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error in GetAllApplications() : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("GetApplicationTypes/{tenantId}")]
        public ActionResult<IEnumerable<ApplicationType>> GetApplicationTypes(int tenantId)
        {
            try
            {
                return Ok(_app.GetApplicationTypes(tenantId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in GetApplicationTypes() : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("AddApplicationToBook/{bookId}/{appIds}")]
        public IActionResult AddApplicationToBook(int bookId, string appIds)
        {
            try
            {
                if (bookId > 0)
                {

                    int[] ApplicationIds = Array.ConvertAll(appIds.Split(','), int.Parse);
                    var insertedRows = _app.AddApplications(bookId, ApplicationIds);

                    if (insertedRows > 0)
                    {
                        return Ok($"Inserted ${insertedRows} rows");
                    }
                    else
                    {
                        return Ok("Failed to insert");
                    }
                }
                else
                {
                    _logger.LogError($"Invalid BookId : {bookId} or ApplicationIds : {appIds} in AddApplications");
                    return BadRequest($"Invalid BookId : {bookId} or ApplicationIds : {appIds}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in AddApplication() : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("GetBookApplications/{bookId}")]
        public ActionResult<IEnumerable<Application>> GetApplicationByBookId(int bookId)
        {
            try
            {
                if (bookId > 0)
                {
                    return Ok(_app.GetApplicationByBookId(bookId));
                }
                else
                {
                    _logger.LogError($"Invalid BookId in GetApplicationsByBookId() : {bookId}");
                    return BadRequest($"Invalid BookId in : {bookId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in GetApplicationsByBookId() : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("CreateApplicationType/{tenantId}")]
        public IActionResult CreateApplicationType([FromBody] ApplicationType appType, int tenantId)
        {
            try
            {
                if (!string.IsNullOrEmpty(appType.AppTypeName) && tenantId > 0)
                {
                    var res = _app.CreateCustomApplicationType(appType, tenantId);

                    if (res > 0)
                    {
                        return Ok("Application Type created successfully");
                    }
                    else
                    {
                        return BadRequest("Application Type Not created");
                    }
                }
                else
                {
                    _logger.LogError($"Empty Application name : {appType} Or invalid tenantId : {tenantId} in CreateApplicationType");
                    return BadRequest($"Empty Application name : {appType} Or invalid tenantId : {tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in CreateApplicationType : {ex} ");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
