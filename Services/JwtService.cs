using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
//using WebApi.Services.Interface;

namespace WebApi.Services
{
    public class JwtService
    {
        private readonly string _key;

        public JwtService(string key)
        {
            if (key.Length < 32)
            {
                throw new ArgumentException("The secret key must be at least 32 characters long.");
            }
            _key = key;
        }

        public string GenerateToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
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
    }
    //public class JwtService
    //{
    //    private readonly string _jwtKey;

    //    public JwtService(string jwtKey)
    //    {
    //        _jwtKey = jwtKey;
    //    }
    //    public string GenerateToken(AppGlobalVariableService globalVariableService)
    //    {
    //        var claims = new[]
    //        {
    //            new Claim(JwtRegisteredClaimNames.Sub, globalVariableService.UserName ?? ""),
    //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //            new Claim("UserID", globalVariableService.UserID ?? ""),
    //            new Claim("UserName", globalVariableService.UserName ?? ""),
    //            new Claim("UserDisplayName", globalVariableService.UserDisplayName ?? ""),
    //            new Claim("UserRoleID", globalVariableService.UserRoleID ?? ""),
    //            new Claim("UserRoleName", globalVariableService.UserRoleName ?? ""),
    //            new Claim("UserRoleType", globalVariableService.UserRoleType ?? ""),
    //            new Claim("DisplayPDPA", globalVariableService.DisplayPDPA ?? ""),
    //            new Claim("PayrollAccessible", globalVariableService.PayrollAccessible ?? ""),
    //            new Claim("UserAccessRightsCount", globalVariableService.UserAccessRightsCount ?? ""),
    //            new Claim("SetPassword", globalVariableService.SetPassword ?? ""),
    //            new Claim("PasswordExpiry", globalVariableService.PasswordExpiry ?? ""),
    //            new Claim("ValidateOTP", globalVariableService.ValidateOTP ?? ""),
    //            new Claim("IsProfileUser", globalVariableService.IsProfileUser ?? ""),
    //            new Claim("IsSystemUser", globalVariableService.IsSystemUser ?? ""),
    //            new Claim("UserImgPath", globalVariableService.UserImgPath ?? ""),
    //            new Claim("DBName", globalVariableService.DBName ?? ""),
    //            new Claim("DBChange", globalVariableService.DBChange ?? ""),
    //            new Claim("GlobalUser", globalVariableService.GlobalUser ?? ""),
    //            new Claim("InstanceChange", globalVariableService.InstanceChange ?? ""),
    //            new Claim("InstanceName", globalVariableService.InstanceName ?? ""),
    //            new Claim("DataBaseUserName", globalVariableService.DataBaseUserName ?? ""),
    //            new Claim("DataBasePassword", globalVariableService.DataBasePassword ?? ""),
    //            new Claim("TimeOffset", globalVariableService.TimeOffset ?? ""),
    //            new Claim("TimeZoneID", globalVariableService.TimeZoneID ?? ""),
    //            new Claim("IsAppuserTimeZone", globalVariableService.IsAppuserTimeZone ?? ""),
    //            new Claim("LanguageCode", globalVariableService.LanguageCode ?? "")
    //        };

    //        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtKey));
    //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    //        var token = new JwtSecurityToken(
    //            //_issuer,
    //            //_audience,
    //           // claims,
    //            expires: DateTime.Now.AddHours(1),
    //            signingCredentials: creds
    //        );

    //        return new JwtSecurityTokenHandler().WriteToken(token);
    //    }

    //    public ClaimsPrincipal ValidateToken(string token)
    //    {
    //        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtKey));
    //        var handler = new JwtSecurityTokenHandler();

    //        var tokenValidationParameters = new TokenValidationParameters
    //        {
    //            ValidateIssuer = true,
    //            ValidateAudience = true,
    //            ValidateLifetime = true,
    //            ValidateIssuerSigningKey = true,
    //            //ValidIssuer = _issuer,
    //            //ValidAudience = _audience,
    //            IssuerSigningKey = key
    //        };

    //        try
    //        {
    //            var principal = handler.ValidateToken(token, tokenValidationParameters, out _);
    //            return principal;
    //        }
    //        catch
    //        {
    //            return null; // Handle invalid token
    //        }
    //    }
    //}

}
