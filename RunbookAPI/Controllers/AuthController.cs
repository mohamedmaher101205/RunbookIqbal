using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;

namespace Runbook.API.Controllers
{
    /// <summary>
    /// This AuthController class have methods to performing singup registration and 
    /// login the user
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly ILogger _logger;

        /// <summary>
        /// This contructor is to inject object using dependency injection
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="logger"></param>
        public AuthController(IAuthService auth, ILogger<AuthController> logger)
        {
            _auth = auth;
            _logger = logger;
        }

        /// <summary>
        /// Authenticate the user and returns the token
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Token</returns>       
        /// <returns>Success message</returns>
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

        /// <summary>
        /// Register the sign up user and returns success message
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Success message</returns>
        [HttpPost]
        [Route("Register")]
        public IActionResult RegisterUser([FromBody] User user)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.UserEmail))
                {
                    var UserRegistration = _auth.RegisterUser(user);

                    if (UserRegistration == "User exist")
                    {
                        return StatusCode(206, "User with same email already exist");
                    }
                    else if (UserRegistration == "successfull")
                    {
                        return Ok("User registered successfully");
                    }
                    else
                        return StatusCode(206, UserRegistration);
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