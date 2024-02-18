namespace Sa.Login.Api.Models;

public class Mail
{
    public required string ToEmail { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
}