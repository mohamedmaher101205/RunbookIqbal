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
    /// This ResourceController class have methods to performing create a resource, create a resource type
    /// get all resource,get all resource type
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceService _resource;
        private readonly ILogger _logger;

        /// <summary>
        /// This contructor is to inject object using dependency injection
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="resource"></param>
        public ResourceController(ILogger<ResourceController> logger, IResourceService resource)
        {
            _logger = logger;
            _resource = resource;
        }

        /// <summary>
        /// create a resource
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="tenantId"></param>
        /// <returns>Success message</returns>
        [HttpPost]
        [Route("CreateResourceType/{tenantId}")]
        public IActionResult CreateResourceType([FromBody] ResourceType resourceType, int tenantId)
        {
            try
            {
                if (!string.IsNullOrEmpty(resourceType.ResourceTypeName) && tenantId > 0)
                {
                    var res = _resource.CreateResourceType(resourceType, tenantId);

                    if (res > 0)
                    {
                        return Ok("Resource Type created successfully");
                    }
                    else
                    {
                        return BadRequest("Resource Type Not created");
                    }
                }
                else
                {
                    _logger.LogError($"Empty ResourceType name : {resourceType} Or invalid tenantId : {tenantId} in CreateResourceType");
                    return BadRequest($"Empty ResourceType name : {resourceType} Or invalid tenantId : {tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in CreateResourceType : {ex} ");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Read all resource list
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of resource type</returns>
        [HttpGet]
        [Route("GetResourceTypes/{tenantId}")]
        public ActionResult<IEnumerable<ResourceType>> GetAllResourceTypes(int tenantId)
        {
            try
            {
                if (tenantId > 0)
                {
                    return Ok(_resource.GetResourceTypes(tenantId));
                }
                else
                {
                    _logger.LogError($"Invalid TenantId in GetAllResourceTypes : {tenantId}");
                    return BadRequest($"Invalid TenantId : {tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error in GetAllResourceTypes() : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// create a resource
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="tenantId"></param>
        /// <returns>Success message</returns>
        [HttpPost]
        [Route("CreateResource/{tenantId}")]
        public IActionResult CreateResource([FromBody] Resource resource, int tenantId)
        {
            try
            {
                if (!string.IsNullOrEmpty(resource.ResourceName) && tenantId > 0)
                {
                    var res = _resource.CreateResource(resource, tenantId);

                    if (res > 0)
                    {
                        return Ok("Resource created successfully");
                    }
                    else
                    {
                        return BadRequest("Resource Not created");
                    }
                }
                else
                {
                    _logger.LogError($"Empty Application name : {resource} in CreateResource");
                    return BadRequest($"Empty Application name : {resource}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in CreateResource : {ex} ");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Read all resource list
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of resource</returns>
        [HttpGet]
        [Route("GetResources/{tenantId}")]
        public ActionResult<IEnumerable<Resource>> GetAllResources(int tenantId)
        {
            try
            {
                if (tenantId > 0)
                {
                    return Ok(_resource.GetAllResources(tenantId));
                }
                else
                {
                    _logger.LogError($"Invalid TenantId in GetAllResources : {tenantId}");
                    return BadRequest($"Invalid TenantId : {tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error in GetAllResources() : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
