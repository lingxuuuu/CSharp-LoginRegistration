using Microsoft.EntityFrameworkCore;

namespace loginreg.Models
{
    public class LoginContext : DbContext
    {
        public LoginContext(DbContextOptions options) : base(options) { }

        public DbSet<User> User { get; set; }
    }
}