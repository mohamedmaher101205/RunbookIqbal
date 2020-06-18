using System.Collections;
using System;
using System.Text;
using System.Security.Claims;
using RunbookAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;

namespace RunbookAPI.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private IConfiguration _config;

        public JwtTokenGenerator(IConfiguration config)
        {
            _config = config;
        }

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
                new Claim("family_name",user.LastName)
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"], 
                _config["Jwt:Issuer"], 
                claims,
                expires:DateTime.Now.AddMinutes(30),
                signingCredentials:credential
                );

            return new AuthRequest{
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresIn = token.ValidTo
            };
        }
    }
}