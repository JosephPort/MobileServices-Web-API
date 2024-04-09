using Microsoft.EntityFrameworkCore;
using MobileServices_Web_API.Models;

namespace MobileServices_Web_API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; } = null!;
    }
}
