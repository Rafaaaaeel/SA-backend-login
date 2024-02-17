namespace Sa.Login.Api.Data;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }
    
    public DbSet<User> User { get; set; }

    public async Task<User?> QueryUserByEmail(string email)
    {
        return await User.FirstOrDefaultAsync(u => u.Email == email);
    }
}

