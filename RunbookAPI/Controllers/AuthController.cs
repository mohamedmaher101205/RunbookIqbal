using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using Microsoft.AspNetCore.Http;
using Runbook.API.Templates;

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

        private readonly IMailService _mail;

        /// <summary>
        /// This contructor is to inject object using dependency injection
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="logger"></param>
        public AuthController(IAuthService auth, ILogger<AuthController> logger, IMailService mail)
        {
            _auth = auth;
            _logger = logger;
            _mail = mail;
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
                if (!string.IsNullOrEmpty(user.UserEmail))
                {
                    IActionResult response = Unauthorized();
                    AuthRequest token = null;

                    if (string.IsNullOrEmpty(user.Password))
                    {
                        token = _auth.OpenIdAuthenticateUser(user);
                    }
                    else
                    {
                        token = _auth.AuthenticateUser(user);
                    }

                    if (token.Token != null)
                    {
                        response = Ok(token);
                    }

                    return response;
                }
                else
                {
                    _logger.LogError($"User email is empty : {user}");
                    return BadRequest("Email should not be empty");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error in Login : {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
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
                if (!string.IsNullOrEmpty(user.UserEmail) && !string.IsNullOrEmpty(user.Password))
                {
                    var UserRegistration = _auth.RegisterUser(user, out string msg);

                    if (msg == "User exist")
                    {
                        return Conflict("User with same email already exist");
                        //return StatusCode(StatusCodes.Status206PartialContent,"User with same email already exist");
                    }
                    else if (msg == "successfull")
                    {
                        return Ok(UserRegistration);
                    }
                    else
                        return StatusCode(StatusCodes.Status500InternalServerError, UserRegistration);
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server Error");
            }
        }

        [HttpPost]
        [Route("ForgotPasswordSendOTP")]
        public IActionResult ForgotPasswordSendOTP([FromBody] User user)
        {
            try
            {
                ///user.UserEmail = "anishetty6666@gmail.com";
                IActionResult response = Unauthorized();
                bool isExistingUser;

                isExistingUser = _auth.checkExistingUser(user);


                if (isExistingUser is true)
                {
                    string OTP = _auth.OTPGenrate();
                    if (!string.IsNullOrEmpty(OTP))
                    {
                        string body = OneTimePasswordTemplate.emailTemplate;

                        body = body.Replace("{OTP}", OTP);
                        string subject = "One-time password";
                        _mail.SendEmail(user.UserEmail, subject, body);
                    }

                    response = Ok(OTP);
                }
                else{
                    return BadRequest("User email doesn't exist");
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
        [Route("ResetPassword")]
        public IActionResult ResetPassword([FromBody] User user)
        {

            try
            {
                if (!string.IsNullOrEmpty(user.UserEmail))
                {
                    var UserRegistration = _auth.ResetPassword(user);

                    if (UserRegistration == "UserNotExist")
                    {
                        return BadRequest("User Not exist");
                    }
                    else if (UserRegistration == "successfull")
                    {
                        return Ok("Password Changed successfully");
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