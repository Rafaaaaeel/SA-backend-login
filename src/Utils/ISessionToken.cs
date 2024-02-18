namespace LoginApp.Utils;

public interface ISessionToken 
{
    string CreateToken(User user);
    bool CheckTokenIsValid(string token);
}