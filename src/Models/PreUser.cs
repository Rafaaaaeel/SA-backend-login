namespace LoginApp.Models;

public class PreUser 
{
    public string? Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public int Token { get; set; } = new Random().Next(1000, 9000);
}