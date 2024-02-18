namespace Sa.Login.Api.Request;

public class RegisterRequest
{
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}