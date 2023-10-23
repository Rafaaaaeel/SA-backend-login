using LoginApp.Models;
using LoginApp.Dtos;

namespace LoginApp.Services 
{
    public interface IAuthService 
    {
        Task<AuthResponse<PreUser>> Register(RegisterDto request);
        Task<AuthResponse<User>> Login(LoginDto request);
        AuthResponse<RefreshTokenDto> RefreshToken(Token token);
    }
}