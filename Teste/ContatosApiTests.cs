using ContatosListMinimalAPI.Core;
using ContatosListMinimalAPI.Infraestructure.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;

[TestFixture]
public class ContatosApiTests
{
    [Test]
    public void CriarContato_DeveRetornarContatoCriado()
    {
        // Arrange
        var mockDbSet = new Mock<DbSet<Contato>>();
        var mockDbContext = new Mock<AppDbContext>();
        mockDbContext.Setup(c => c.Contatos).Returns(mockDbSet.Object);

        var service = new ContatosService(mockDbContext.Object);
        var novoContato = new Contato { Nome = "Teste", Telefone = "123456789", Email = "teste@teste.com" };

        // Act
        var resultado = service.CriarContato(novoContato);

        // Assert
        Assert.NotNull(resultado);
        Assert.AreEqual(novoContato.Nome, resultado.Nome);
        Assert.AreEqual(novoContato.Telefone, resultado.Telefone);
        Assert.AreEqual(novoContato.Email, resultado.Email);
    }

    [Test]
    public void ObterContatos_DeveRetornarListaDeContatos()
    {
        // Arrange
        var contatos = new List<Contato>
        {
            new Contato { Id = 1, Nome = "Teste1", Telefone = "123456789", Email = "teste1@teste.com" },
            new Contato { Id = 2, Nome = "Teste2", Telefone = "987654321", Email = "teste2@teste.com" }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Contato>>();
        mockDbSet.As<IQueryable<Contato>>().Setup(m => m.Provider).Returns(contatos.Provider);
        mockDbSet.As<IQueryable<Contato>>().Setup(m => m.Expression).Returns(contatos.Expression);
        mockDbSet.As<IQueryable<Contato>>().Setup(m => m.ElementType).Returns(contatos.ElementType);
        mockDbSet.As<IQueryable<Contato>>().Setup(m => m.GetEnumerator()).Returns(contatos.GetEnumerator());

        var mockDbContext = new Mock<AppDbContext>();
        mockDbContext.Setup(c => c.Contatos).Returns(mockDbSet.Object);

        var service = new ContatosService(mockDbContext.Object);

        // Act
        var resultado = service.ObterContatos();

        // Assert
        Assert.NotNull(resultado);
        Assert.AreEqual(contatos.Count(), resultado.Count());
    }

    // Implemente outros testes para AtualizarContato, ExcluirContato, etc., seguindo a mesma lógica.
}
