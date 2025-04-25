using Microsoft.EntityFrameworkCore;
using quachdaihiep_th2.Models;

namespace quachdaihiep_th2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; } // Định nghĩa bảng trong DB
        public DbSet<Category> Categories { get; set; }

    }
}
