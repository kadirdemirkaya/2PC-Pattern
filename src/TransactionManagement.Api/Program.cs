using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TransactionManagement.Api.Model;
using TransactionManagement.Api.Services;
using TransactionManagement.Api.Services.Author;
using TransactionManagement.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("AuthorApi", client => client.BaseAddress = new("https://localhost:7154/"));
builder.Services.AddHttpClient("BookApi", client => client.BaseAddress = new("https://localhost:7179/"));

builder.Services.AddDbContext<TwoPhaseCommitDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.AddScoped<IAuthorTransactionService, AuthorTransactionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/create-author-transaction", async (HttpContext context) =>
{
    var _sp = builder.Services.BuildServiceProvider();
    var transactionService = _sp.GetRequiredService<IAuthorTransactionService>();

    var createAuthorModel = await JsonSerializer.DeserializeAsync<CreateAuthorModel>(context.Request.Body);

    string[] names = { "Author.Api", "Book.Api" };
    Guid transactionId = await transactionService.CreateTransactionAsync(Guid.NewGuid(), names);
    await transactionService.PrepareServices(transactionId);
    bool transactionState = await transactionService.CheckReadyServices(transactionId);

    if (transactionState)
    {
        await transactionService.Commit(transactionId, createAuthorModel);
        transactionState = await transactionService.CheckTransactionStateServices(transactionId);
    }

    if (!transactionState)
        await transactionService.Rollback(transactionId, createAuthorModel);

    return true;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
