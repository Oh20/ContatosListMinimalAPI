using ContatosListMinimalAPI.Core;
using ContatosListMinimalAPI.Infraestructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

// Adicionar serviços de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ContactList Minimal API", Version = "v1" });
});

var app = builder.Build();

// Configurar middleware do Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContactList V1");
        c.RoutePrefix = string.Empty;
    });
}

// Configurar o pipeline de requisição HTTP.
app.UseHttpsRedirection();

// Definir endpoints
app.MapPost("/contatos", async (Contato contato, AppDbContext db) =>
{
    if (!MinimalValidation.TryValidate(contato, out var errors))
    {
        return Results.BadRequest(errors);
    }

    db.Contatos.Add(contato);

    try
    {
        await db.SaveChangesAsync();
        return Results.Created($"/contatos/{contato.Id}", contato);
    }
    catch (DbUpdateException ex)
    {
        return Results.BadRequest(new { error = "Nome ou telefone já está em uso" });
    }
})
.WithName("CriarContato")
.WithTags("Contatos");

app.MapGet("/contatos", async (AppDbContext db) =>
{
    return Results.Ok(await db.Contatos.ToListAsync());
})
.WithName("ObterContatos")
.WithTags("Contatos");

app.MapGet("/contatos/ddd/{ddd}", async (string ddd, AppDbContext db) =>
{
    var contatos = await db.Contatos.Where(c => c.Telefone.StartsWith(ddd)).ToListAsync();
    return Results.Ok(contatos);
})
.WithName("ObterContatosPorDdd")
.WithTags("Contatos");

app.MapPut("/contatos/{id}", async (int id, Contato contatoAtualizado, AppDbContext db) =>
{
    var contato = await db.Contatos.FindAsync(id);
    if (contato is null) return Results.NotFound();

    contato.Nome = contatoAtualizado.Nome;
    contato.Telefone = contatoAtualizado.Telefone;
    contato.Email = contatoAtualizado.Email;

    if (!MinimalValidation.TryValidate(contato, out var errors))
    {
        return Results.BadRequest(errors);
    }

    try
    {
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (DbUpdateException ex)
    {
        return Results.BadRequest(new { error = "Nome ou telefone já está em uso" });
    }
})
.WithName("AtualizarContato")
.WithTags("Contatos");

app.MapDelete("/contatos/{id}", async (int id, AppDbContext db) =>
{
    var contato = await db.Contatos.FindAsync(id);
    if (contato is null) return Results.NotFound();

    db.Contatos.Remove(contato);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("ExcluirContato")
.WithTags("Contatos");

app.Run();
