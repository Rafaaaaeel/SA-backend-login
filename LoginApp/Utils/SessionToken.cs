using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using LoginApp.Models;

namespace LoginApp.Utils;

public class SessionToken : ISessionToken
{
    private readonly IConfiguration _configuration;

    public SessionToken(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim> 
        {
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public bool CheckTokenIsValid(string token)
    {
        var tokenTicks = GetTokenExpirationTime(token);
        var tokenDate = DateTimeOffset.FromUnixTimeSeconds(tokenTicks).UtcDateTime;

        var now = DateTime.Now.ToUniversalTime();

        var valid = tokenDate >= now;

        return valid;
    }

    private long GetTokenExpirationTime(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
        var ticks= long.Parse(tokenExp);
        return ticks;
    }
}