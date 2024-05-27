using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;

public class ContatosApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ContatosApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private HttpClient GetClient()
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove o AppDbContext existente
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Adiciona um AppDbContext com um banco de dados em memória
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Opcional: Configurar outros serviços específicos de teste aqui
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetContatos_ReturnsEmptyList_WhenNoContatos()
    {
        var client = GetClient();

        var response = await client.GetAsync("/contatos");

        response.EnsureSuccessStatusCode();
        var contatos = await response.Content.ReadFromJsonAsync<List<Contato>>();
        Assert.Empty(contatos);
    }

    [Fact]
    public async Task PostContato_CreatesNewContato()
    {
        var client = GetClient();
        var novoContato = new Contato { Nome = "Teste", Telefone = "123456789", Email = "teste@teste.com" };

        var response = await client.PostAsJsonAsync("/contatos", novoContato);

        response.EnsureSuccessStatusCode();
        var createdContato = await response.Content.ReadFromJsonAsync<Contato>();
        Assert.NotNull(createdContato);
        Assert.Equal(novoContato.Nome, createdContato.Nome);
    }

    [Fact]
    public async Task PutContato_UpdatesExistingContato()
    {
        var client = GetClient();
        var novoContato = new Contato { Nome = "Teste", Telefone = "123456789", Email = "teste@teste.com" };

        var postResponse = await client.PostAsJsonAsync("/contatos", novoContato);
        postResponse.EnsureSuccessStatusCode();
        var createdContato = await postResponse.Content.ReadFromJsonAsync<Contato>();

        createdContato.Nome = "Teste Atualizado";
        var putResponse = await client.PutAsJsonAsync($"/contatos/{createdContato.Id}", createdContato);

        putResponse.EnsureSuccessStatusCode();
        var updatedContato = await client.GetFromJsonAsync<Contato>($"/contatos/{createdContato.Id}");
        Assert.Equal("Teste Atualizado", updatedContato.Nome);
    }

    [Fact]
    public async Task DeleteContato_RemovesContato()
    {
        var client = GetClient();
        var novoContato = new Contato { Nome = "Teste", Telefone = "123456789", Email = "teste@teste.com" };

        var postResponse = await client.PostAsJsonAsync("/contatos", novoContato);
        postResponse.EnsureSuccessStatusCode();
        var createdContato = await postResponse.Content.ReadFromJsonAsync<Contato>();

        var deleteResponse = await client.DeleteAsync($"/contatos/{createdContato.Id}");

        deleteResponse.EnsureSuccessStatusCode();
        var getResponse = await client.GetAsync($"/contatos/{createdContato.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
