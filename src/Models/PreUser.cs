namespace Sa.Login.Api.Models;
public class PreUser 
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public int Token { get; set; } = new Random().Next(1000, 9000);
}