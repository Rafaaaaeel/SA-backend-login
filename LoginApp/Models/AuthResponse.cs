namespace LoginApp.Models
{
    public class AuthResponse<T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
        public SessionToken? Token { get; set; }
        public bool? Error { get; set; }
    }
}