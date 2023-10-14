using LoginApp.Models;
using LoginApp.Dtos;

namespace LoginApp.Services 
{
    public interface IAuthService 
    {
        Task<AuthResponse<User>> Register(RegisterDto request);
        Task<AuthResponse<User>> Login(LoginDto request);
    }
}