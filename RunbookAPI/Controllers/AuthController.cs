using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.API.Templates;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;

namespace Runbook.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly ILogger _logger;
         private readonly IMailService _mail;

        public AuthController(IAuthService auth, ILogger<AuthController> logger,IMailService mail)
        {
            _auth = auth;
            _logger = logger;
             _mail = mail;
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

                    if(UserRegistration == "User exist")
                    {
                        return StatusCode(206,"User with same email already exist");
                    }
                    else if(UserRegistration == "successfull")
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

        [HttpPost]
        [Route("ForgotPasswordSendOTP")]
        public IActionResult ForgotPasswordSendOTP([FromBody] User user)
        {
            try
            {   
               ///user.UserEmail = "anishetty6666@gmail.com";
                IActionResult response = Unauthorized();
                bool isExistingUser ;

                isExistingUser = _auth.checkExistingUser(user);
                

                if (isExistingUser is true)
                {
                  string OTP =  _auth.OTPGenrate();
                  if (!string.IsNullOrEmpty(OTP))
                  {   
                       string body = EmailTemplate.OneTimePasswordTemplate(OTP);
                       string subject = "One-time password";
                      _mail.SendEmail(user.UserEmail, subject, body);
                  }
                  
                  response = Ok(OTP);
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

                    if(UserRegistration == "UserNotExist")
                    {
                        return StatusCode(206,"User Not exist");
                    }
                    else if(UserRegistration == "successfull")
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