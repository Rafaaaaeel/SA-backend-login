namespace Sa.Login.Api.Profiles;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegisterRequest, PreUser>();
        CreateMap<PreUser, RegisterRequest>();
        CreateMap<PreUser, User>();
    }
}
