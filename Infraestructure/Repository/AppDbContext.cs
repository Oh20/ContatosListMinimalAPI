using Core;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repository
{
    public class AppDbContext : DbContext
    {
        public DbSet<Contato> Contatos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Contato>().ToTable("Contatos");
        }
    }
}