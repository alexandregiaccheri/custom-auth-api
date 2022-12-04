using CustomAuthApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomAuthApi.Data
{
    public class CustomAuthDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public CustomAuthDbContext(DbContextOptions<CustomAuthDbContext> options)
            : base(options) { }
    }
}
