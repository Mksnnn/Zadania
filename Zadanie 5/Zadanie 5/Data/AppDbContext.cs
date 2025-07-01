using Microsoft.EntityFrameworkCore;
using Zadanie_5.Models;

namespace Zadanie_5.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
        public DbSet<Klient> Klienci { get; set; }
    }
}
