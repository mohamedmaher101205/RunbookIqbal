using System.Collections;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RunbookAPI.Models;
using RunbookAPI.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using RunbookAPI.Filters;

namespace RunbookAPI.Controllers
{
    [Authorize]
    [ApiController]
    [RefreshToken]
    [Route("[controller]")]
    public class ApplicationController : ControllerBase
    {
        private IApplicationService _app;
        private ILogger _logger;

        public ApplicationController(ILogger<ApplicationController> logger,IApplicationService app)
        {
            _logger = logger;
            _app = app;
        }

        [HttpPost]
        [Route("createapp/{tenantId}")]
        public IActionResult CreateApplication([FromBody] Application app,int tenantId)
        {
            try{
                if(!string.IsNullOrEmpty(app.ApplicationName))
                {
                    var currentUser = HttpContext.User;
                    //int tenantId=int.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "TenantId").Value);

                    var res = _app.CreateApplication(app,tenantId);

                    if(res){
                        return Ok("Application created successfully");
                    }
                    else{
                        return BadRequest("Application Not created");
                    }
                }
                else{
                    _logger.LogError($"Empty Application name : {app} in CreateApplication");
                    return BadRequest($"Empty Application name : {app}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error in CreateApplication : {ex} ");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpGet]
        [Route("applications/{tenantId}")]
        public ActionResult<IEnumerable<Application>> GetAllApplications(int tenantId)
        {
            try{
                if(tenantId > 0){
                    return Ok(_app.GetAllApplications(tenantId));
                }
                else{
                    _logger.LogError($"Invalid TenantId in GetAllApplications : {tenantId}");
                    return BadRequest($"Invalid TenantId : {tenantId}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal server error in GetAllApplications() : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpGet]
        [Route("applicationtypes/{tenantId}")]
        public ActionResult<IEnumerable<ApplicationType>> GetApplicationTypes(int tenantId)
        {
            try{
                return Ok(_app.GetApplicationTypes(tenantId));
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error in GetApplicationTypes() : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpPost]
        [Route("addapplications/{bookId}/{appIds}")]
        public IActionResult AddApplications(int bookId,string appIds)
        {
            try{
                if(bookId > 0 ){

                    int[] ApplicationIds = Array.ConvertAll(appIds.Split(','), int.Parse);
                    var insertedRows =  _app.AddApplications(bookId,ApplicationIds);

                    if(insertedRows > 0){
                        return Ok($"Inserted ${insertedRows} rows");
                    }
                    else{
                        return Ok("Failed to insert");
                    }
                }
                else{
                    _logger.LogError($"Invalid BookId : {bookId} or ApplicationIds : {appIds} in AddApplications");
                    return BadRequest($"Invalid BookId : {bookId} or ApplicationIds : {appIds}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error in AddApplication() : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }
        
        [HttpGet]
        [Route("bookapplications/{bookId}")]
        public ActionResult<IEnumerable<Application>> GetApplicationByBookId(int bookId)
        {
            try{
                if(bookId > 0){
                    return Ok(_app.GetApplicationByBookId(bookId));
                }
                else{
                    _logger.LogError($"Invalid BookId in GetApplicationsByBookId() : {bookId}");
                    return BadRequest($"Invalid BookId in : {bookId}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error in GetApplicationsByBookId() : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpPost]
        [Route("createapptype/{tenantId}")]
        public IActionResult CreateApplicationType([FromBody] ApplicationType appType,int tenantId)
        {
            try{
                if(!string.IsNullOrEmpty(appType.AppTypeName) && tenantId > 0)
                {
                    var res = _app.CreateCustomApplicationType(appType,tenantId);

                    if(res > 0){
                        return Ok("Application Type created successfully");
                    }
                    else{
                        return BadRequest("Application Type Not created");
                    }
                }
                else{
                    _logger.LogError($"Empty Application name : {appType} Or invalid tenantId : {tenantId} in CreateApplicationType");
                    return BadRequest($"Empty Application name : {appType} Or invalid tenantId : {tenantId}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error in CreateApplicationType : {ex} ");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpPost]
        [Route("createresourcetype/{tenantId}")]
        public IActionResult CreateResourceType([FromBody] ResourceType resourceType,int tenantId)
        {
            try{
                if(!string.IsNullOrEmpty(resourceType.ResourceTypeName) && tenantId > 0)
                {
                    var res = _app.CreateResourceType(resourceType,tenantId);

                    if(res > 0){
                        return Ok("Resource Type created successfully");
                    }
                    else{
                        return BadRequest("Resource Type Not created");
                    }
                }
                else{
                    _logger.LogError($"Empty ResourceType name : {resourceType} Or invalid tenantId : {tenantId} in CreateResourceType");
                    return BadRequest($"Empty ResourceType name : {resourceType} Or invalid tenantId : {tenantId}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error in CreateResourceType : {ex} ");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpGet]
        [Route("resourcetypes/{tenantId}")]
        public ActionResult<IEnumerable<ResourceType>> GetAllResourceTypes(int tenantId)
        {
            try{
                if(tenantId > 0){
                    return Ok(_app.GetResourceTypes(tenantId));
                }
                else{
                    _logger.LogError($"Invalid TenantId in GetAllResourceTypes : {tenantId}");
                    return BadRequest($"Invalid TenantId : {tenantId}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal server error in GetAllResourceTypes() : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpPost]
        [Route("createresource/{tenantId}")]
        public IActionResult CreateResource([FromBody] Resource resource,int tenantId)
        {
            try{
                if(!string.IsNullOrEmpty(resource.ResourceName) && tenantId > 0)
                {
                    var res = _app.CreateResource(resource,tenantId);

                    if(res > 0){
                        return Ok("Resource created successfully");
                    }
                    else{
                        return BadRequest("Resource Not created");
                    }
                }
                else{
                    _logger.LogError($"Empty Application name : {resource} in CreateResource");
                    return BadRequest($"Empty Application name : {resource}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error in CreateResource : {ex} ");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpGet]
        [Route("resources/{tenantId}")]
        public ActionResult<IEnumerable<Resource>> GetAllResources(int tenantId)
        {
            try{
                if(tenantId > 0){
                    return Ok(_app.GetAllResources(tenantId));
                }
                else{
                    _logger.LogError($"Invalid TenantId in GetAllResources : {tenantId}");
                    return BadRequest($"Invalid TenantId : {tenantId}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal server error in GetAllResources() : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }
    }
}
