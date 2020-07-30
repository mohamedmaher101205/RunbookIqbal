using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.API.Filters;
using Runbook.API.Templates;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runbook.API.Controllers
{
    [Authorize]
    [ApiController]
    [RefreshToken]
    [Route("[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IGroupService _group;

        public GroupController(ILogger<GroupController> logger, IGroupService group)
        {
            _logger = logger;
            _group = group;
        }

        [HttpPost]
        [Route("CreateGroup/{tenantId}")]
        public ActionResult CreateGroup([FromBody] Group group, int tenantId)
        {
            try
            {
                if (tenantId > 0 && !string.IsNullOrEmpty(group.GroupName))
                {
                    var res = _group.CreateGroup(tenantId, group);
                    if (res > 0)
                    {
                        return Ok("Group created successfully");
                    }
                    else
                    {
                        return Ok("Group Creation unsuccessfull");
                    }
                }
                else
                {
                    _logger.LogError($"Invalid tenantId : {tenantId} or GroupName : {group.GroupName} in CreateGroup");
                    return BadRequest($"Invalid tenantId : {tenantId} or GroupName : {group.GroupName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error in CreateGroup : {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        [Route("GetPermissions")]
        public ActionResult<IEnumerable<Permissions>> GetPermissions()
        {
            try
            {
                var res = _group.GetPermissions();
                if (res != null)
                {
                    return Ok(res);
                }
                else
                {
                    return Ok($"No Permissions found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error in GetPermissions : {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        [Route("GetGroups/{tenantId}")]
        public ActionResult<IEnumerable<Group>> GetTenantGroups(int tenantId)
        {
            try
            {
                if (tenantId > 0)
                {
                    var res = _group.GetTenantGroups(tenantId);
                    if (res != null)
                    {
                        return Ok(res);
                    }
                    else
                    {
                        return Ok($"No Groups for TenantId : {tenantId}");
                    }
                }
                else
                {
                    _logger.LogError($"Invalid TenantId : {tenantId} in GetTenantGroups");
                    return BadRequest($"Invalid TenantId : {tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error in GetTenantGroups : {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Route("AddGroupUsers/{groupId}/{userIds}")]
        public IActionResult AddUsersToGroup(int groupId, string userIds)
        {
            try
            {
                if (groupId > 0)
                {
                    int[] UserIds = Array.ConvertAll(userIds.Split(','), int.Parse);
                    int res = _group.AddUsersToGroup(groupId, UserIds);
                    if (res > 0)
                    {
                        return Ok($"{res} rows inserted");
                    }
                    return Ok($"rows failed to insert");
                }
                else
                {
                    _logger.LogError($"Invalid GroupId in AddUsersToGroup : {groupId}");
                    return BadRequest($"Invalid GroupId : {groupId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server Error in AddUsersToGroup : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("GetGroupUsers/{groupId}")]
        public ActionResult<IEnumerable<User>> GetGroupUsers(int groupId)
        {
            try
            {
                if (groupId > 0)
                {
                    var response = _group.GetGroupUsers(groupId);
                    if (response != null)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return Ok("No users found for this Group");
                    }
                }
                else
                {
                    _logger.LogError($"Invalid tenantId in GetGroupUsers : {groupId}");
                    return BadRequest($"Invalid tenantId : {groupId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in GetGroupUsers : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}