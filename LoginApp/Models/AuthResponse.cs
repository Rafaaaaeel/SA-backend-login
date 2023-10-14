namespace LoginApp.Models
{
    public class AuthResponse<T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
    }
}