namespace Sa.Login.Api.Interfaces;

public interface IAuthService 
{
    Task Register(RegisterRequest request);
    Task<Token> Login(LoginRequest request);
    Task Confirm(int token);
    RefreshTokenResponse RefreshToken(Token token);
}