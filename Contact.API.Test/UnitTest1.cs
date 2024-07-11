using ContatosListMinimalAPI.Core;
using ContatosListMinimalAPI.Infraestructure.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Text.Json;

namespace ContatosListMinimalAPI.Tests
{
    [TestFixture]
    public class ContatosApiTests
    {
        private IHost _host;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseTestServer();
                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddDbContext<AppDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("InMemoryDbForTesting");
                        });

                        services.AddEndpointsApiExplorer();
                        services.AddSwaggerGen(c =>
                        {
                            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "ContactList Minimal API", Version = "v1" });
                        });
                    });
                    webBuilder.Configure(app =>
                    {
                        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

                        if (env.IsDevelopment())
                        {
                            app.UseSwagger();
                            app.UseSwaggerUI(c =>
                            {
                                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContactList V1");
                                c.RoutePrefix = string.Empty;
                            });
                        }

                        app.UseHttpsRedirection();

                        // Configure endpoints as in the original application
                        app.UseRouting();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapPost("/contatos", async context =>
                            {
                                var db = context.RequestServices.GetRequiredService<AppDbContext>();
                                var contato = await JsonSerializer.DeserializeAsync<Contato>(context.Request.Body);

                                if (!MinimalValidation.TryValidate(contato, out var errors))
                                {
                                    context.Response.StatusCode = 400;
                                    await context.Response.WriteAsJsonAsync(errors);
                                    return;
                                }

                                db.Contatos.Add(contato);

                                try
                                {
                                    await db.SaveChangesAsync();
                                    context.Response.StatusCode = 201;
                                    context.Response.Headers["Location"] = $"/contatos/{contato.Id}";
                                    await context.Response.WriteAsJsonAsync(contato);
                                }
                                catch (DbUpdateException)
                                {
                                    context.Response.StatusCode = 400;
                                    await context.Response.WriteAsJsonAsync(new { error = "Nome ou telefone j� est� em uso" });
                                }
                            });

                            endpoints.MapGet("/contatos", async context =>
                            {
                                var db = context.RequestServices.GetRequiredService<AppDbContext>();
                                var contatos = await db.Contatos.ToListAsync();
                                await context.Response.WriteAsJsonAsync(contatos);
                            });

                            endpoints.MapGet("/contatos/ddd/{ddd}", async context =>
                            {
                                var db = context.RequestServices.GetRequiredService<AppDbContext>();
                                var ddd = (string)context.Request.RouteValues["ddd"];
                                var contatos = await db.Contatos.Where(c => c.Telefone.StartsWith(ddd)).ToListAsync();
                                await context.Response.WriteAsJsonAsync(contatos);
                            });

                            endpoints.MapPut("/contatos/{id}", async context =>
                            {
                                var db = context.RequestServices.GetRequiredService<AppDbContext>();
                                var id = int.Parse((string)context.Request.RouteValues["id"]);
                                var contatoAtualizado = await JsonSerializer.DeserializeAsync<Contato>(context.Request.Body);
                                var contato = await db.Contatos.FindAsync(id);
                                if (contato is null)
                                {
                                    context.Response.StatusCode = 404;
                                    return;
                                }

                                contato.Nome = contatoAtualizado.Nome;
                                contato.Telefone = contatoAtualizado.Telefone;
                                contato.Email = contatoAtualizado.Email;

                                if (!MinimalValidation.TryValidate(contato, out var errors))
                                {
                                    context.Response.StatusCode = 400;
                                    await context.Response.WriteAsJsonAsync(errors);
                                    return;
                                }

                                try
                                {
                                    await db.SaveChangesAsync();
                                    context.Response.StatusCode = 204;
                                }
                                catch (DbUpdateException)
                                {
                                    context.Response.StatusCode = 400;
                                    await context.Response.WriteAsJsonAsync(new { error = "Nome ou telefone j� est� em uso" });
                                }
                            });

                            endpoints.MapDelete("/contatos/{id}", async context =>
                            {
                                var db = context.RequestServices.GetRequiredService<AppDbContext>();
                                var id = int.Parse((string)context.Request.RouteValues["id"]);
                                var contato = await db.Contatos.FindAsync(id);
                                if (contato is null)
                                {
                                    context.Response.StatusCode = 404;
                                    return;
                                }

                                db.Contatos.Remove(contato);
                                await db.SaveChangesAsync();
                                context.Response.StatusCode = 204;
                            });
                        });
                    });
                })
                .Start();

            _client = _host.GetTestClient();
        }

        [TearDown]
        public void Teardown()
        {
            _client.Dispose();
            _host.Dispose();
        }

        [Test]
        public async Task Test_CreateContato_ReturnsCreated()
        {
            var contato = new Contato { Nome = "Teste", Telefone = "123456789", Email = "teste@example.com" };
            //var contato = new Contato { Nome = "", Telefone = "123456789", Email = "teste@example.com" }; // Passando nome Vazio
            //var contato = new Contato { Nome = "Teste", Telefone = "", Email = "teste@example.com" }; // Passando telefone vazio
            var content = new StringContent(JsonSerializer.Serialize(contato), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/contatos", content);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var createdContato = JsonSerializer.Deserialize<Contato>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.IsNotNull(createdContato);
            Assert.AreEqual("Teste", createdContato.Nome);
        }

        [Test]
        public async Task Test_GetContatos_ReturnsOk()
        {
            var response = await _client.GetAsync("/contatos");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var contatos = JsonSerializer.Deserialize<List<Contato>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.IsNotNull(contatos);
            Assert.IsInstanceOf<List<Contato>>(contatos);
        }

        [Test]
        public async Task Test_UpdateContato_ReturnsNoContent()
        {
            var contato = new Contato { Nome = "Teste", Telefone = "123456789", Email = "teste@example.com" };
            var content = new StringContent(JsonSerializer.Serialize(contato), Encoding.UTF8, "application/json");

            var createResponse = await _client.PostAsync("/contatos", content);
            createResponse.EnsureSuccessStatusCode();

            var createdContato = JsonSerializer.Deserialize<Contato>(await createResponse.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var updatedContato = new Contato { Nome = "Teste Atualizado", Telefone = "987654321", Email = "testeatualizado@example.com" };
            content = new StringContent(JsonSerializer.Serialize(updatedContato), Encoding.UTF8, "application/json");

            var updateResponse = await _client.PutAsync($"/contatos/{createdContato.Id}", content);

            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, updateResponse.StatusCode);
        }

        [Test]
        public async Task Test_DeleteContato_ReturnsNoContent()
        {
            var contato = new Contato { Nome = "Teste", Telefone = "123456789", Email = "teste@example.com" };
            var content = new StringContent(JsonSerializer.Serialize(contato), Encoding.UTF8, "application/json");

            var createResponse = await _client.PostAsync("/contatos", content);
            createResponse.EnsureSuccessStatusCode();

            var createdContato = JsonSerializer.Deserialize<Contato>(await createResponse.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var deleteResponse = await _client.DeleteAsync($"/contatos/{createdContato.Id}");

            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }
    }
}
