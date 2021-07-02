using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        // Na osnovu ovog polja zna koje tabele treba da kreira u bazi
        public DbSet<Character> Characters { get; set; }

        public DbSet<User> Users { get; set; }
    }
}