using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Runbook.Models;
using Runbook.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Runbook.API.Filters
{
    public class RefreshTokenAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var config = context.HttpContext.RequestServices.GetService<IConfiguration>();

            if (!string.IsNullOrEmpty(context.HttpContext.Request.Headers["Authorization"].ToString()))
            {
                var requestAuthHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
                string[] authToken = requestAuthHeader.Split(" ");
                var token = authToken[1];
                var handler = new JwtSecurityTokenHandler();
                var tokenString = handler.ReadJwtToken(token) as JwtSecurityToken;
                long expireTime = long.Parse(tokenString.Claims.FirstOrDefault(claim => claim.Type == "exp").Value);

                long currentTime = (long)DateTime.Now.ToUniversalTime().Subtract(
                    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                ).TotalSeconds;


                if ((expireTime - currentTime) <= 500)
                {
                    User user = new User
                    {
                        FirstName = tokenString.Claims.FirstOrDefault(claim => claim.Type == "given_name").Value,
                        LastName = tokenString.Claims.FirstOrDefault(claim => claim.Type == "family_name").Value,
                        UserId = int.Parse(tokenString.Claims.FirstOrDefault(claim => claim.Type == "UserId").Value),
                        TenantId = int.Parse(tokenString.Claims.FirstOrDefault(claim => claim.Type == "TenantId").Value),
                        UserEmail = tokenString.Claims.FirstOrDefault(claim => claim.Type == "email").Value
                    };

                    if (string.IsNullOrEmpty(context.HttpContext.Response.Headers["Authorization"]))
                    {
                        JwtTokenGenerator jwt = new JwtTokenGenerator(config);
                        AuthRequest newToken = jwt.GenerateToken(user);
                        context.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Authorization");
                        context.HttpContext.Response.Headers.Add("Authorization", newToken.Token);
                    }
                }
            }
        }
    }
}