using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.API.Templates;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runbook.API.Controllers
{
    /// <summary>
    /// This UserController class have methods to performing to select particular tenant, 
    /// get all users,send email to the user
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _user;
        private readonly ILogger _logger;
        private readonly IMailService _mail;

        /// <summary>
        /// This contructor is to inject object using dependency injection
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="user"></param>
        /// <param name="mail"></param>
        public UserController(ILogger<UserController> logger, IUserService user, IMailService mail)
        {
            _logger = logger;
            _user = user;
            _mail = mail;
        }

        /// <summary>
        /// Get Tenant information by TenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTenant/{tenantId}")]
        public ActionResult<Tenant> GetTenant(int tenantId)
        {
            try
            {
                if (tenantId > 0)
                {
                    var response = _user.GetTenant(tenantId);
                    if (response != null)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(201, "No Tenants for this userId");
                    }
                }
                else
                {
                    _logger.LogError($"Invalid UserId : {tenantId} in GetTenant");
                    return BadRequest($"Invalid UserId : {tenantId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in GetTenant : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Send email to invite the user
        /// </summary>
        /// <param name="email"></param>
        /// <returns>sucess message</returns>
        [HttpPost]
        [Route("InviteUserByEmail")]
        public async Task<IActionResult> SendEMail([FromBody] InviteUsers inviteUsers)
        {
            try
            {              

                var res = _user.CreateInviteUsers(inviteUsers);

                if (res)
                {
                    string subject = "Invitation For RunBook Application";
                    string body = InviteUserTemplate.emailTemplate;
                    body = body.Replace("{UserName}", inviteUsers.UserName);
                     body = body.Replace("{InviteRoleLevel}", inviteUsers.InviteRoleLevel);
                    //System.IO.File.ReadAllText("./Templates/InviteUserTemplate.html");
                    _logger.LogInformation("Preparing an Email");

                    await _mail.SendEmail(inviteUsers.InviteUserEmailId, subject, body);
                    _logger.LogInformation("Email sent");
                    return Ok("Email sent successfully");
                }
                else
                {
                    return Ok("User already Exist or Already Invitation Sent");
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error in SendEMail : {ex}");
                return StatusCode(500, $"Error while sending Email: {ex}");
            }
        }

        /// <summary>
        /// read all users list
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>list of users</returns>
        [HttpGet]
        [Route("GetUsers/{tenantId}")]
        public ActionResult<IEnumerable<User>> GetAllUsers(int tenantId)
        {
            try
            {
                if (tenantId > 0)
                {
                    var response = _user.GetAllUsers(tenantId);
                    if (response != null)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        _logger.LogError("Empty response in GetAllUsers");
                        return Ok(null);
                    }
                }
                else
                {
                    _logger.LogError($"Invalid tenantId in GetAllUsers : {tenantId}");
                    return BadRequest($"Invalid tenantId : {tenantId}");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in GetAllUsers : {ex}");
                return StatusCode(500, "Internal server Error");
            }
        }
    }
}