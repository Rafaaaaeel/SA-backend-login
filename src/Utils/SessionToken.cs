namespace LoginApp.Utils;

public class SessionToken : ISessionToken
{
    private readonly string _key;

    public SessionToken(AppSettings appSettings)
    {
        _key = appSettings.JwtConfiguration.IssuerSigningKey;
    }

    public string CreateToken(User user)
    {
        List<Claim> claims = [new(ClaimTypes.Email, user.Email)];

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_key));

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