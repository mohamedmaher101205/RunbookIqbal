using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Runbook.Models;
using Runbook.Services;
using Runbook.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Runbook.API.Helpers
{
    /// <summary>
    /// this class is to handle authenication for the user
    /// </summary>
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        User _user = null;

        /// <summary>
        /// contructor is to get user details object
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        /// <param name="userService"></param>
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService userService)
            : base(options, logger, encoder, clock)
        {
            _user = new User();
        }

        /// <summary>
        /// This method is to validate token for authentication
        /// </summary>
        /// <returns>authentication result</returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            bool verified = false;
            try
            {
                verified = await System.Threading.Tasks.Task.Run(() => ValidateJWTToken());
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (!verified)
                return AuthenticateResult.Fail("Invalid Username or Password");

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, _user.UserId.ToString()),
                new Claim(ClaimTypes.Email, _user.UserEmail),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        /// <summary>
        /// This method is to validate token
        /// </summary>
        /// <returns>true or false</returns>
        private async Task<bool> ValidateJWTToken()
        {
            try
            {
                var config = Context.RequestServices.GetService<IConfiguration>();

                if (!string.IsNullOrEmpty(Context.Request.Headers["Authorization"].ToString()))
                {
                    var requestAuthHeader = Context.Request.Headers["Authorization"].ToString();
                    string[] authToken = requestAuthHeader.Split(" ");
                    var token = authToken[1];
                    var handler = new JwtSecurityTokenHandler();
                    var tokenString = handler.ReadJwtToken(token) as JwtSecurityToken;
                    long expireTime = long.Parse(tokenString.Claims.FirstOrDefault(claim => claim.Type == "exp").Value);

                    long currentTime = (long)DateTime.Now.ToUniversalTime().Subtract(
                        new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    ).TotalSeconds;


                    _user.FirstName = tokenString.Claims.FirstOrDefault(claim => claim.Type == "given_name").Value;
                    _user.LastName = tokenString.Claims.FirstOrDefault(claim => claim.Type == "family_name").Value;
                    _user.UserId = int.Parse(tokenString.Claims.FirstOrDefault(claim => claim.Type == "UserId").Value);
                    _user.TenantId = int.Parse(tokenString.Claims.FirstOrDefault(claim => claim.Type == "TenantId").Value);
                    _user.UserEmail = tokenString.Claims.FirstOrDefault(claim => claim.Type == "email").Value;

                    if ((expireTime - currentTime) <= 500)
                    {

                        if (string.IsNullOrEmpty(Context.Response.Headers["Authorization"]))
                        {
                            JwtTokenGenerator jwt = new JwtTokenGenerator(config);
                            AuthRequest newToken = jwt.GenerateToken(_user);
                            Context.Response.Headers.Add("Access-Control-Allow-Headers", "Authorization");
                            Context.Response.Headers.Add("Authorization", newToken.Token);
                        }
                    }
                }
            }
            catch { return false; }
            return true;
        }
    }
}


