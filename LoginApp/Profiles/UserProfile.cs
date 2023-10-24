using AutoMapper;
using LoginApp.Dtos;
using LoginApp.Models;

namespace LoginApp.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterDto, User>();
            CreateMap<RegisterDto, PreUser>();
            CreateMap<PreUser, RegisterDto>();
        }
    }

}