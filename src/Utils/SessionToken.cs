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
        List<Claim> claims = [new(ClaimTypes.Email, user.Email)];

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512Signature);

        JwtSecurityToken token = new(
            claims: claims,
            expires: DateTime.Now.AddMonths(1),
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

    private static long GetTokenExpirationTime(string token)
    {
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(token);
        var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
        var ticks = long.Parse(tokenExp);
        return ticks;
    }
}