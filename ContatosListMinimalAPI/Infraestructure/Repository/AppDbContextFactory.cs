using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ContatosListMinimalAPI.Infraestructure.Repository
{
    //Criamos o ContextFactory para facilitar a criação de instancias do DbContext através do EFcore
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Encontra o caminho da raiz do projeto
            var basePath = Directory.GetCurrentDirectory();

            // Ajusta o caminho para o diretório do projeto de inicialização
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("ConnectionString");
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
