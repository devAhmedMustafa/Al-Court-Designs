using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using OrdrMate.Enums;

namespace OrdrMate.Middlewares;

public class JWTService(IConfiguration c)
{

    private readonly IConfiguration _config = c;

    public string GenerateJWT(string userId, UserRole role)
    {

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: [
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role.ToString()),
            ],
            expires: DateTime.Now.AddHours(720), // 30 days
            notBefore: DateTime.Now,
            signingCredentials: cred
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}