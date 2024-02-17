namespace Sa.Login.Api.Repositories;

public class AuthService : IAuthService
{
    private readonly UserContext _context;
    private readonly IMapper _mapper;
    private readonly ISessionToken _token;
    private readonly IEmailSender _email;

    public AuthService(UserContext context, IMapper mapper, ISessionToken token, IEmailSender email)
    {
        _context = context;
        _mapper = mapper;
        _token = token;
        _email = email;
    }

    // Ajustar para funcionar com Redis
    public async Task<AuthResponse<PreUser>> Register(RegisterDto request)
    {
        var isUserAlreadyInUse = await _context.User.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (isUserAlreadyInUse != null) return new AuthResponse<PreUser>() { Error = true, Message = "User Already exist", Code = 409 };

        var preUser = _mapper.Map<PreUser>(request);

        preUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password); 

        // var response = await SavePreUser(preUser);
        
        SendEmail(preUser);

        return new AuthResponse<PreUser>();
    }

    public async Task<Token> Login(LoginRequest request) 
    {
        User? user = await _context.QueryUserByEmail(request.Email) ?? throw new ArgumentException();

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) throw new UnathorizedException();

        Token token = new() { Value = _token.CreateToken(user)};
        
        return token;
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
        return new AuthResponse<PreUser>();
    }

    private void SendEmail(PreUser user)
    {
        Mail request = new() { ToEmail = user.Email, Body = $"<a href='http://localhost:5124/api/Auth/confirm/{user.Id}'> Test {user.Token} <a/>", Subject = $"Account confirmation SA"};
        _email.SendEmail(request);
    }

}