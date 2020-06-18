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
    public class UserController : ControllerBase
    {
        private IUserService _user;
        private ILogger _logger;

        public UserController(ILogger<UserController> logger,IUserService user)
        {
            _logger = logger;
            _user = user;
        }

        [HttpPost]
        [Route("addusers/{tenantId}/{userIds}")]
        public IActionResult LinkUsers(int tenantId,string userIds)
        {
            try
            {
                if(tenantId > 0){
                    int[] UserIds = Array.ConvertAll(userIds.Split(','), int.Parse);
                    int res = _user.LinkUsers(tenantId,UserIds);
                    if(res > 0){
                        return Ok($"{res} rows inserted");
                    }
                    return Ok($"rows failed to insert");
                }
                else{
                    _logger.LogError($"Invalid TenantId in LinkUsers : {tenantId}");
                    return BadRequest($"Invalid TenantId : {tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server Error in LinkUsers : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpGet]
        [Route("linkedusers/{tenantId}")]
        public ActionResult<IEnumerable<User>> GetLinkedUsersForTenant(int tenantId)
        {
            try{
                if(tenantId > 0){
                    var response = _user.GetLinkedUsers(tenantId);
                    if(response != null){
                        return Ok(response);
                    }
                    else{
                        return Ok("No linked users found for this tenant");
                    }
                }
                else{
                    _logger.LogError($"Invalid tenantId in GetLinkedUsersForTenant : {tenantId}");
                    return BadRequest($"Invalid tenantId : {tenantId}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error in GetLinkedUsersForTenant : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }
        
        [HttpPost]
        [Route("group/{tenantId}")]
        public ActionResult CreateGroup([FromBody] Group group,int tenantId)
        {
            try{
                if(tenantId > 0 && !string.IsNullOrEmpty(group.GroupName)){
                    var res = _user.CreateGroup(tenantId,group);
                    if(res > 0){
                        return Ok("Group created successfully");
                    }
                    else{
                        return Ok("Book Creation unsuccessfull");
                    }
                }
                else{
                    _logger.LogError($"Invalid tenantId : {tenantId} or GroupName : {group.GroupName} in CreateGroup");
                    return BadRequest($"Invalid tenantId : {tenantId} or GroupName : {group.GroupName}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal server error in CreateGroup : {ex}");
                return StatusCode(500,"Internal server error");
            }
        }

        [HttpGet]
        [Route("groups/{tenantId}")]
        public ActionResult<IEnumerable<Group>> GetTenantGroups(int tenantId)
        {
            try{
                if(tenantId > 0){
                    var res = _user.GetTenantGroups(tenantId);
                    if(res != null){
                        return Ok(res);
                    }
                    else{
                        return Ok($"No Groups for TenantId : {tenantId}");
                    }
                }
                else{
                    _logger.LogError($"Invalid TenantId : {tenantId} in GetTenantGroups");
                    return BadRequest($"Invalid TenantId : {tenantId}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal server error in GetTenantGroups : {ex}");
                return StatusCode(500,"Internal server error");
            }
        }

        [HttpPost]
        [Route("addgroupusers/{groupId}/{userIds}")]
        public IActionResult AddUsersToGroup(int groupId,string userIds)
        {
            try
            {
                if(groupId > 0){
                    int[] UserIds = Array.ConvertAll(userIds.Split(','), int.Parse);
                    int res = _user.AddUsersToGroup(groupId,UserIds);
                    if(res > 0){
                        return Ok($"{res} rows inserted");
                    }
                    return Ok($"rows failed to insert");
                }
                else{
                    _logger.LogError($"Invalid GroupId in AddUsersToGroup : {groupId}");
                    return BadRequest($"Invalid GroupId : {groupId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server Error in AddUsersToGroup : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpGet]
        [Route("groupusers/{groupId}")]
        public ActionResult<IEnumerable<User>> GetGroupUsers(int groupId)
        {
            try{
                if(groupId > 0){
                    var response = _user.GetGroupUsers(groupId);
                    if(response != null){
                        return Ok(response);
                    }
                    else{
                        return Ok("No users found for this Group");
                    }
                }
                else{
                    _logger.LogError($"Invalid tenantId in GetGroupUsers : {groupId}");
                    return BadRequest($"Invalid tenantId : {groupId}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error in GetGroupUsers : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpGet]
        [Route("tenant/{tenantId}")]
        public ActionResult<Tenant> GetTenant(int tenantId)
        {
            try{
                if(tenantId > 0){
                    var response = _user.GetTenant(tenantId);
                    if(response != null){
                        return Ok(response);
                    }
                    else{
                        return StatusCode(201,"No Tenants for this userId");
                    }
                }else{
                    _logger.LogError($"Invalid UserId : {tenantId} in GetTenant");
                    return BadRequest($"Invalid UserId : {tenantId}");
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error in GetTenant : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpPost]
        [Route("customenvironments/{tenantId}")]
        public IActionResult CreateCustomEnvironment([FromBody] Environments env,int tenantId)
        {
            try
            {
                if(tenantId > 0){
                    int res = _user.CreateEnvironment(env,tenantId);
                    if(res > 0){
                        return Ok($"{res} Environments inserted");
                    }
                    return Ok($"Environments failed to insert");
                }
                else{
                    _logger.LogError($"Invalid TenantId in CreateCustomEnvironment : {tenantId}");
                    return BadRequest($"Invalid TenantId : {tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server Error in CreateCustomEnvironment : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }
    }
}
