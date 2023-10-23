namespace LoginApp.Models; 
public class AuthResponse<T>
{
    public T? Data { get; set; }
    public string? Message { get; set; }
    public Token? Token { get; set; }
    public bool? Error { get; set; }
    public int? Code { get; set;}
}
