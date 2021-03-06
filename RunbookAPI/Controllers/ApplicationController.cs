using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using Microsoft.AspNetCore.Http;

namespace Runbook.API.Controllers
{
    /// <summary>
    /// This ApplicationController class used to create Application and tenant, Read particular application or all, Read particular 
    /// application type or all, add application to book, read application using book
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _app;
        private readonly ILogger _logger;

        /// <summary>
        /// This contructor is to inject object using dependency injection
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="app"></param>
        public ApplicationController(ILogger<ApplicationController> logger, IApplicationService app)
        {
            _logger = logger;
            _app = app;
        }

        /// <summary>
        /// Creates the application for a tenant
        /// </summary>
        /// <param name="app"></param>
        /// <param name="tenantId"></param>
        /// <returns>Success message</returns>
        /// <respone code="200">Returns successfull message if application created successfully</respone>
        /// <response code="400">Returns Bad request if error occurs while creating application</response>
        /// <response code="500">Returns Internal server error, if any error occurs</response>
        [HttpPost]
        [Route("CreateApplication/{tenantId}")]
        public IActionResult CreateApplication([FromBody] Application app, int tenantId)
        {
            try
            {
                if (!string.IsNullOrEmpty(app.ApplicationName) && tenantId > 0)
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
                    _logger.LogError($"Empty Application name : {app} or Invalid TenantId : {tenantId} in CreateApplication");
                    return BadRequest($"Empty Application name : {app} or Invalid TenantId : {tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in CreateApplication : {ex} ");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        /// <summary>
        /// Get all the applications for a tenant by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of all application</returns>
        /// <response code="200">Return list of applications</response>
        /// <response code="400">If tenantId is invalid</response>
        [HttpGet]
        [Route("GetApplications/{tenantId}")]
        public IActionResult GetAllApplications(int tenantId)
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        /// <summary>
        /// Get the application types available for a tenant by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of all application types</returns>
        [HttpGet]
        [Route("GetApplicationTypes/{tenantId}")]
        public IActionResult GetApplicationTypes(int tenantId)
        {
            try
            {
                if(tenantId > 0)
                {
                    return Ok(_app.GetApplicationTypes(tenantId));
                }
                else
                {
                    _logger.LogInformation($"Invalid TenantId : ${tenantId} in GetAllApllications");
                    return BadRequest($"Invalid TenantId : ${tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in GetApplicationTypes() : {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        /// <summary>
        /// Adds the application to the book using ApplicationId and BookId
        /// </summary>
        /// <param name="bookId"></param>   
        /// <param name="appIds"></param>
        /// <returns>Numbers of rows inserted</returns>
        [HttpPost]
        [Route("AddApplicationToBook/{bookId}/{appIds}")]
        public IActionResult AddApplicationToBook(int bookId, string appIds)
        {
            try
            {
                if (bookId > 0 && !string.IsNullOrEmpty(appIds))
                {
                    int[] ApplicationIds = Array.ConvertAll(appIds.Split(','), int.Parse);
                    var insertedRows = _app.AddApplications(bookId, ApplicationIds);
                    var book = _app.GetBookForMultipleRelease(bookId);                    
                    if (insertedRows > 0)
                    {
                        return Ok($"Inserted ${insertedRows} rows");
                        _app.SendMailMultipleresourceOnSameDate(book);
                    } 
                    else
                    {
                        return NotFound("Failed to insert");
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        /// <summary>
        /// Get applications that are added to book by passing parameter bookId
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns>List of all application</returns>
        [HttpGet]
        [Route("GetBookApplications/{bookId}")]
        public IActionResult GetApplicationByBookId(int bookId)
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        /// <summary>
        /// Create the application type for a tenant
        /// </summary>
        /// <param name="appType"></param>
        /// <param name="tenantId"></param>
        /// <returns>Success message</returns>
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
