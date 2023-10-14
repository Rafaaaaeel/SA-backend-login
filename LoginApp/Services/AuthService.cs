using AutoMapper;
using Azure;
using LoginApp.Data;
using LoginApp.Dtos;
using LoginApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginApp.Services
{
    public class AuthService : IAuthService
    {

        private readonly UserContext _db;
        private readonly IMapper _mapper;

        public AuthService(UserContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
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

            if (user.Email != request.Email) return new AuthResponse<User>();

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return new AuthResponse<User>();
            
            return new AuthResponse<User>() { Data = user };
        }


    }
}