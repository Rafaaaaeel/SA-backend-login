namespace LoginApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Vip { get; set; } = false;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    }
}