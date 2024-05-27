using ContatosListMinimalAPI.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

[Fact]
public void GetContatos_ShouldReturnListOfContacts()
{
    // Arrange
    var factory = new WebApplicationFactory<Startup>();
    var client = factory.CreateClient();

    // Act
    var response = client.GetAsync("/contatos");

    // Assert
    response.EnsureSuccessStatusCode(); // Check status code
    var contacts = response.Content.ReadFromJsonAsync<List<Contato>>().Result;
    Assert.NotEmpty(contacts); // Check if contacts list is not empty
}
