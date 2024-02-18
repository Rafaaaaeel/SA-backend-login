namespace Sa.Login.Api.Repositories;

public class AuthService : IAuthService
{
    private readonly UserContext _context;
    private readonly IMapper _mapper;
    private readonly ISessionToken _token;
    private readonly IClientCache _redis;

    public AuthService(UserContext context, IMapper mapper, ISessionToken token, IClientCache redis)
    {
        _context = context;
        _mapper = mapper;
        _token = token;
        _redis = redis;
    }

    public async Task Register(RegisterRequest request)
    {
        if (await _context.QueryUserByEmail(request.Email) is not null) throw new ConflictException();

        var preUser = _mapper.Map<PreUser>(request);

        preUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password); 

        await _redis.AsyncSetCacheObject(preUser.Token.ToString(), preUser);
    }

    public async Task<Token> Login(LoginRequest request) 
    {
        User user = await _context.QueryUserByEmail(request.Email) ?? throw new NotFoundException();

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) throw new UnathorizedException();

        Token token = new() { Value = _token.CreateToken(user)};
        
        return token;
    }

    public async Task Confirm(int token)
    {
        PreUser preUser = await _redis.AsyncGetCachedObject<PreUser>(token.ToString());

        User user = _mapper.Map<User>(preUser);

        _context.Add(user);

        await _context.SaveChangesAsync();
    }

    public RefreshTokenResponse RefreshToken(Token token)
    {
        if (token.Value is null) throw new NotFoundException();

        bool valid = _token.CheckTokenIsValid(token.Value);

        if (!valid) throw new GoneException();

        return new RefreshTokenResponse() { Valid = valid };
    }

}