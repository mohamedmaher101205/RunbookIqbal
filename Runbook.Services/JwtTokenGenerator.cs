using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Runbook.Services
{
    /// <summary>
    /// This JwtTokenGenerator class is to generate the token for user
    /// </summary>
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// This constructor is to inject IConfiguration using constructor dependency injuction
        /// </summary>
        /// <param name="config"></param>
        public JwtTokenGenerator(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// generate the token for user 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public AuthRequest GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("given_name",user.FirstName),
                new Claim("UserId",user.UserId.ToString()),
                new Claim("TenantId",user.TenantId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.UserEmail),
                new Claim("family_name",user.LastName),
                new Claim("IsAdmin",user.IsAdmin.ToString()),
                new Claim("Permissions",JsonConvert.SerializeObject(user.Permissions))
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credential
                );

            return new AuthRequest
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresIn = token.ValidTo
            };
        }
    }
}