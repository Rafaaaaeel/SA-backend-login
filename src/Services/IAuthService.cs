namespace Sa.Login.Api.Interfaces;

public interface IAuthService 
{
    Task Register(RegisterRequest request);
    Task<Token> Login(LoginRequest request);
    RefreshTokenResponse RefreshToken(Token token);
    Task Confirm(int token);
}