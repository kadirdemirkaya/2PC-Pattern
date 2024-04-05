using Book.Api.Data;
using Book.Api.Entities;
using Book.Api.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BookDbContext>(o => o.UseInMemoryDatabase("BookDbContext"));

var sp = builder.Services.BuildServiceProvider();
await BookSeedContext.SeedAsync(sp.GetRequiredService<BookDbContext>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};

app.MapGet("/ready", () =>
{
    return true;
});

app.MapPost("/commit", async context =>
{
    var _sp = builder.Services.BuildServiceProvider();
    var _context = _sp.GetRequiredService<BookDbContext>();
    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
    {
        try
        {
            var requestBody = await reader.ReadToEndAsync();
            var books = JsonSerializer.Deserialize<List<Book.Api.Entities.Book>>(requestBody, options);

            await _context.Books.AddRangeAsync(books);
            if (await _context.SaveChangesAsync() > 0)
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync("OK");
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("BadRequest");
            }
        }
        catch
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync("BadRequest");
        }
    }
});

app.MapPost("/rollback", async context =>
{
    var _sp = builder.Services.BuildServiceProvider();
    var _context = _sp.GetRequiredService<BookDbContext>();
    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
    {
        try
        {
            var requestBody = await reader.ReadToEndAsync();
            var books = JsonSerializer.Deserialize<List<Book.Api.Entities.Book>>(requestBody, options);

            bool anyBookExists = await _context.Books.AnyAsync(b => books.Any(book => book.AuthorId == b.AuthorId));

            if (anyBookExists)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsync("NotFound");
            }

            _context.Books.RemoveRange(books);
            if (await _context.SaveChangesAsync() > 0)
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync("OK");
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("BadRequest");
            }
        }
        catch
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync("BadRequest");
            throw;
        }
    }
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
