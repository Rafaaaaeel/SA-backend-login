using AutoMapper;
using LoginApp.Data;
using LoginApp.Dtos;
using LoginApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace LoginApp.Services
{
    public class AuthService : IAuthService
    {

        private readonly UserContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(UserContext db, IMapper mapper, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<AuthResponse<User>> Register(RegisterDto request)
        {
            var isUserAlreadyInUse = await _db.User.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (isUserAlreadyInUse != null) return new AuthResponse<User>();
    
            var user = _mapper.Map<User>(request);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password); 

            await _db.User.AddAsync(user);
            
            await _db.SaveChangesAsync();

            return new AuthResponse<User>() { Data = user };
        }

        public async Task<AuthResponse<User>> Login(LoginDto request) 
        {
            var user = await _db.User.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null) return new AuthResponse<User>();

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return new AuthResponse<User>();

            SessionToken token = new SessionToken() { Token = CreateToken(user)};
            
            return new AuthResponse<User>() { Data = user, Token = token };
        }

        public AuthResponse<RefreshTokenDto> RefreshToken(SessionToken token)
        {
            if (token.Token == null) return new AuthResponse<RefreshTokenDto>();

            bool valid = CheckTokenIsValid(token.Token);

            if (!valid) return new AuthResponse<RefreshTokenDto>() { Error = true } ;

            var response = new RefreshTokenDto() { Valid = valid };
            
            return new AuthResponse<RefreshTokenDto>() { Data = response };
        }

        private string CreateToken(User user)
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

        private long GetTokenExpirationTime(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
            var ticks= long.Parse(tokenExp);
            return ticks;
        }

        private bool CheckTokenIsValid(string token)
        {
            var tokenTicks = GetTokenExpirationTime(token);
            var tokenDate = DateTimeOffset.FromUnixTimeSeconds(tokenTicks).UtcDateTime;

            var now = DateTime.Now.ToUniversalTime();

            var valid = tokenDate >= now;

            return valid;
        }
    }
}