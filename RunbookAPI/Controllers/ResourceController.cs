using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

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
        /// Create a resource
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
                    return BadRequest($"Empty ResourceType name : {resourceType.ResourceTypeName} Or invalid tenantId : {tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in CreateResourceType : {ex} ");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        /// <summary>
        /// Get all resource list
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of resource type</returns>
        [HttpGet]
        [Route("GetResourceTypes/{tenantId}")]
        public IActionResult GetAllResourceTypes(int tenantId)
        {
            try
            {
                if (tenantId > 0)
                {
                    var resourceTypes = _resource.GetResourceTypes(tenantId);
                    if(resourceTypes != null)
                    {
                        return Ok(resourceTypes);
                    }
                    else
                    {
                        _logger.LogInformation($"There are no resource types found for tenantId : {tenantId} in GetAllResourceTYpes");
                        return NotFound($"There are no resource types found for tenantId : {tenantId}");
                    }
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        /// <summary>
        /// Create a resource
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
                    _logger.LogError($"Empty Resource name : {resource.ResourceName} or Invalid TenantId : {tenantId} in CreateResource");
                    return BadRequest($"Empty Resource name : {resource.ResourceName} or Invalid TenantId : {tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in CreateResource : {ex} ");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        /// <summary>
        /// Get all resource list
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of resource</returns>
        [HttpGet]
        [Route("GetResources/{tenantId}")]
        public IActionResult GetAllResources(int tenantId)
        {
            try
            {
                if (tenantId > 0)
                {
                    var resources = _resource.GetAllResources(tenantId);
                    if(resources != null)
                    {
                        return Ok(resources);
                    }
                    else
                    {
                        return NotFound($"No Resources found for the TenantId : {tenantId}");
                    }
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
