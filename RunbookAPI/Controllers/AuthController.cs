using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;

namespace Runbook.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly ILogger _logger;

        public AuthController(IAuthService auth, ILogger<AuthController> logger)
        {
            _auth = auth;
            _logger = logger;
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] User user)
        {
            try
            {
                IActionResult response = Unauthorized();
                object token = null;

                if (string.IsNullOrEmpty(user.Password))
                {
                    token = _auth.OpenIdAuthenticateUser(user);
                }
                else
                {
                    token = _auth.AuthenticateUser(user);
                }

                if (token != null)
                {
                    response = Ok(token);
                }

                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error in Login : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult RegisterUser([FromBody] User user)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.UserEmail))
                {
                    var UserRegistration = _auth.RegisterUser(user);

                    return Ok(UserRegistration);
                }
                else
                {
                    _logger.LogError($"User Email is empty in register user");
                    return BadRequest("User email or password should not be empty");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server error in Register user : {ex}");
                return StatusCode(500, "Internal server Error");
            }
        }
    }
}