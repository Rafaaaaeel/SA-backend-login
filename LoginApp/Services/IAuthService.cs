using LoginApp.Models.Dtos;

namespace LoginApp.Services 
{
    public interface IAuthService 
    {
        Task Register(UserDto user);
    }
}