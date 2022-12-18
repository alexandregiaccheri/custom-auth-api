using CustomAuthApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomAuthApi.Data.Context
{
    public class CustomAuthDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public CustomAuthDbContext(DbContextOptions<CustomAuthDbContext> options)
            : base(options) { }
    }
}
