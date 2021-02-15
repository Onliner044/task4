using Microsoft.EntityFrameworkCore;

namespace task4.Models {
    public class UsersContext : DbContext {
        public DbSet<User> Users { get; set; }

        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options) {
            Database.EnsureCreated();
        }
    }
}
