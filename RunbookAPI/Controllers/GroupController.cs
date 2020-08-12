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
    /// This GroupController class have methods to performing create a group and get users grop,
    /// select all group,select all permission, add user to the group, get tenant group
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IGroupService _group;

        /// <summary>
        /// This contructor is to inject object using dependency injection
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="group"></param>
        public GroupController(ILogger<GroupController> logger, IGroupService group)
        {
            _logger = logger;
            _group = group;
        }

        /// <summary>
        ///  Create a group
        /// </summary>
        /// <param name="group"></param>
        /// <param name="tenantId"></param>
        /// <returns>Success message</returns>
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

        /// <summary>
        /// Read list of permissions
        /// </summary>
        /// <returns>List of permission</returns>
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

        /// <summary>
        ///  Read group list based on tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>Tenant Group list</returns>
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

        /// <summary>
        /// Add users to the group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userIds"></param>
        /// <returns>Receive added users message</returns>
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

        /// <summary>
        /// Get list of users based on group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>list of users</returns>
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