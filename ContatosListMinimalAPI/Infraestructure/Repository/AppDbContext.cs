using ContatosListMinimalAPI.Core;
using Microsoft.EntityFrameworkCore;

namespace ContatosListMinimalAPI.Infraestructure.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Contato> Contatos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contato>()
                .HasIndex(c => c.Nome)
                .IsUnique();

            modelBuilder.Entity<Contato>()
                .HasIndex(c => c.Telefone)
                .IsUnique();

            modelBuilder.Entity<Contato>()
                .Property(c => c.Nome)
                .IsRequired();

            modelBuilder.Entity<Contato>()
                .Property(c => c.Telefone)
                .IsRequired();
        }
    }
}
