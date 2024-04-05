using Author.Api.Data;
using Author.Api.Entities;
using Author.Api.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AuthorDbContext>(o => o.UseInMemoryDatabase("AuthorDbContext"));

var sp = builder.Services.BuildServiceProvider();
await AuthorSeedContext.SeedAsync(sp.GetRequiredService<AuthorDbContext>());

builder.Services.AddHttpClient("TransactionManagementApi", client => client.BaseAddress = new("https://localhost:5056/"));

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
    var _context = _sp.GetRequiredService<AuthorDbContext>();
    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
    {
        try
        {
            var requestBody = await reader.ReadToEndAsync();
            Author.Api.Entities.Author? author = JsonSerializer.Deserialize<Author.Api.Entities.Author>(requestBody, options);

            bool anyAuthorExists = await _context.Authors.AnyAsync(a => a.Name == author.Name);

            if (anyAuthorExists)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("BadRequest");
            }

            await _context.Authors.AddRangeAsync(author);
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
    var _context = _sp.GetRequiredService<AuthorDbContext>();
    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
    {
        try
        {
            var requestBody = await reader.ReadToEndAsync();
            Author.Api.Entities.Author? author = JsonSerializer.Deserialize<Author.Api.Entities.Author>(requestBody, options);

            bool anyAuthorExists = await _context.Authors.AnyAsync(a => a.Name == author.Name);

            if (!anyAuthorExists)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsync("NotFound");
            }

            _context.Authors.Remove(author);
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


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
