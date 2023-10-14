using LoginApp.Models.Dtos;

namespace LoginApp.Services
{
    public class AuthService : IAuthService
    {
        public async Task Register(UserDto user)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            
            throw new NotImplementedException();
        }
    }
}