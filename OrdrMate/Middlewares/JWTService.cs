using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OrdrMate.Middlewares;

public class JWTService(IConfiguration c) {

    private readonly IConfiguration _config = c;

    public string GenerateJWT(string managerName){
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: [new Claim(ClaimTypes.Name, managerName)],
            expires: DateTime.Now.AddHours(1),
            signingCredentials: cred
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}