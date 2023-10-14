using LoginApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginApp.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }
        
        public DbSet<User> User { get; set; }
    }
}