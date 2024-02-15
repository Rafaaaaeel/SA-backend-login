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
        private readonly IEmailSender _email;

        public AuthService(UserContext context, IMapper mapper, MongoDBManager mongo, ISessionToken token, IEmailSender email)
        {
            _context = context;
            _mapper = mapper;
            _mongo = mongo;
            _token = token;
            _email = email;
        }

        public async Task<AuthResponse<PreUser>> Register(RegisterDto request)
        {
            var isUserAlreadyInUse = await _context.User.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (isUserAlreadyInUse != null) return new AuthResponse<PreUser>() { Error = true, Message = "User Already exist", Code = 409 };

            var preUser = _mapper.Map<PreUser>(request);

            preUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password); 

            var response = await SavePreUser(preUser);
            
            SendEmail(preUser);

            return response;
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

            if (!valid) return new AuthResponse<RefreshTokenDto>() { Error = true, Code = 410 } ;

            var response = new RefreshTokenDto() { Valid = valid };
            
            return new AuthResponse<RefreshTokenDto>() { Data = response };
        }

        public async Task<AuthResponse<PreUser>> ConfirmUser(string id, int token)
        {
            var response = await _mongo.DeleteUser(id, token);

            if (response.Error == true || response.Data == null) return response;
        
            var data = _mapper.Map<RegisterDto>(response.Data);

            var user = _mapper.Map<User>(data);

            user.PasswordHash = response.Data.PasswordHash;

            await _context.User.AddAsync(user);

            await _context.SaveChangesAsync();

            return response;
        }

        private async Task<AuthResponse<PreUser>> SavePreUser(PreUser request) 
        {
            return await _mongo.CreatePreUser(request);
        }

        private void SendEmail(PreUser user)
        {
            Mail request = new Mail() { ToEmail = user.Email, Body = $"<a href='http://localhost:5124/api/Auth/confirm/{user.Id}'> Test {user.Token} <a/>", Subject = $"Account confirmation SA"};
            _email.SendEmail(request);
        }

    }

}