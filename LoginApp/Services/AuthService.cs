using AutoMapper;
using LoginApp.Data;
using LoginApp.Dtos;
using LoginApp.Models;
using LoginApp.Utils;
using Microsoft.EntityFrameworkCore;


namespace LoginApp.Services
{
    public class AuthService : IAuthService
    {

        private readonly UserContext _context;
        private readonly IMapper _mapper;
        private readonly MongoDBManager _mongo;
        private readonly ISessionToken _token;

        public AuthService(UserContext context, IMapper mapper, MongoDBManager mongo, ISessionToken token)
        {
            _context = context;
            _mapper = mapper;
            _mongo = mongo;
            _token = token;
        }

        public async Task<AuthResponse<User>> Register(RegisterDto request)
        {
            var isUserAlreadyInUse = await _context.User.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (isUserAlreadyInUse != null) return new AuthResponse<User>();
    
            var user = _mapper.Map<User>(request);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password); 

            await _context.User.AddAsync(user);
            
            await _context.SaveChangesAsync();

            return new AuthResponse<User>() { Data = user };
        }

        public async Task<AuthResponse<User>> Login(LoginDto request) 
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null) return new AuthResponse<User>();

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return new AuthResponse<User>();

            Token token = new Token() { Value = _token.CreateToken(user)};
            
            return new AuthResponse<User>() { Data = user, Token = token };
        }

        public AuthResponse<RefreshTokenDto> RefreshToken(Token token)
        {
            if (token.Value == null) return new AuthResponse<RefreshTokenDto>();

            bool valid = _token.CheckTokenIsValid(token.Value);

            if (!valid) return new AuthResponse<RefreshTokenDto>() { Error = true } ;

            var response = new RefreshTokenDto() { Valid = valid };
            
            return new AuthResponse<RefreshTokenDto>() { Data = response };
        }

    }
}