﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Services.Implementation;
//using WebApi.Services.Interface;

namespace WebApi.Services
{
    public class JwtService
    {
        private readonly string _key;
        private readonly TokenBlacklistService _tokenBlacklistService; // Logout
        public JwtService(string key, TokenBlacklistService tokenBlacklistService)
        {
            if (key.Length < 32)
            {
                throw new ArgumentException("The secret key must be at least 32 characters long.");
            }
            _key = key;
            _tokenBlacklistService = tokenBlacklistService;
        }

        public string GenerateToken(string username,string Guid,string Password)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username,Guid,Password) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256
                )



            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public void ValidateToken(string token, string secretKey)
        {
            var key = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                foreach (var claim in principal.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
            }
        }

        public void InvalidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expiration = jwtToken.ValidTo;

            _tokenBlacklistService.RevokeToken(token, expiration);
        }
    }

}
