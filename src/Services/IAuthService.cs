namespace LoginApp.Services 
{
    public interface IAuthService 
    {
        Task<AuthResponse<PreUser>> Register(RegisterDto request);
        Task<Token> Login(LoginRequest request);
        AuthResponse<RefreshTokenDto> RefreshToken(Token token);
        Task<AuthResponse<PreUser>> ConfirmUser(string id, int token);
    }
}