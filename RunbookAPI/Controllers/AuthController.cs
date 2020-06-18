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

namespace RunbookAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private IAuthService _auth;
        private ILogger _logger;

        public AuthController(IAuthService auth,ILogger<AuthController> logger)
        {
            _auth = auth;
            _logger = logger;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] User user)
        {
            try{
                IActionResult response = Unauthorized();
                object token = null;
                
                if(string.IsNullOrEmpty(user.Password)){
                    token = _auth.OpenIdAuthenticateUser(user);
                }
                else{
                    token = _auth.AuthenticateUser(user);
                }

                if(token != null)
                {
                    response = Ok(token);
                }
                
                return response;

            }catch(Exception ex){
                _logger.LogError($"Internal server error in Login : {ex}");
                return StatusCode(500,"Internal Server Error");
            }
        }

        [HttpPost]
        [Route("register")]
        public IActionResult RegisterUser([FromBody] User user)
        {
            try{
                if(!string.IsNullOrEmpty(user.UserEmail))
                {
                    bool isRegistered = _auth.RegisterUser(user);

                    if(isRegistered){
                        return Ok("User registered successufully");
                    }
                    else{
                        return BadRequest("Something went wrong. User registration failed");
                    }
                }
                else{
                    _logger.LogError($"User Email is empty in register user");
                    return BadRequest("User email or password should not be empty");
                }
            }catch(Exception ex){
                _logger.LogError($"Internal Server error in Register user : {ex}");
                return StatusCode(500,"Internal server Error");
            }
        }
    }
}
