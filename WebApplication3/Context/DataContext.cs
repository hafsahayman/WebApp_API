using Microsoft.EntityFrameworkCore;
using WebApplication3.Model;

namespace WebApplication3.Context
{
    public class DataContext :DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Title> Title { get; set; }
        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<Ingredients> Ingredients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<User> User { get; set; }
       // public DbSet<RefresherToken> RefresherToken { get; set; }


    }
}
