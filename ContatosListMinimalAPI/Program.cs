using Core;
using Infraestructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/contacts", async (Contato contact, AppDbContext db) =>
{
    db.Contatos.Add(contact);
    await db.SaveChangesAsync();
    return Results.Created($"/contacts/{contact.Id}", contact);
});

app.MapGet("/contacts", async (AppDbContext db) =>
{
    return Results.Ok(await db.Contatos.ToListAsync());
});

app.MapGet("/contacts/ddd/{ddd}", async (string ddd, AppDbContext db) =>
{
    var contacts = await db.Contatos.Where(c => c.Phone.StartsWith(ddd)).ToListAsync();
    return Results.Ok(contacts);
});

app.MapPut("/contacts/{id}", async (int id, Contato updatedContact, AppDbContext db) =>
{
    var contact = await db.Contatos.FindAsync(id);
    if (contact is null) return Results.NotFound();

    contact.Nome = updatedContact.Nome;
    contact.Phone = updatedContact.Phone;
    contact.Email = updatedContact.Email;
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/contacts/{id}", async (int id, AppDbContext db) =>
{
    var contact = await db.Contatos.FindAsync(id);
    if (contact is null) return Results.NotFound();

    db.Contatos.Remove(contact);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.Run();
