using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TransactionManagement.Api.Model;
using TransactionManagement.Data;
using TransactionManagement.Enums;

namespace TransactionManagement.Api.Services.Author
{
    public class AuthorTransactionService(IHttpClientFactory _httpClientFactory, TwoPhaseCommitDbContext _context) : TransactionService(_httpClientFactory, _context), IAuthorTransactionService
    {
        HttpClient _httpClientAuthorApi = _httpClientFactory.CreateClient("AuthorApi");
        HttpClient _httpClientBookApi = _httpClientFactory.CreateClient("BookApi");

        public async Task Commit(Guid transactionId, CreateAuthorModel createAuthorModel)
        {
            var transactionNodes = await _context.NodeStates
               .Include(ns => ns.Node)
               .Where(ns => ns.TransactionId == transactionId)
               .ToListAsync();

            var jsonAuthorContent = JsonSerializer.Serialize(createAuthorModel.Author);
            var jsonBookContents = JsonSerializer.Serialize(createAuthorModel.Books);

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var authorData = new StringContent(jsonAuthorContent, Encoding.UTF8, "text/plain");
                    var bookData = new StringContent(jsonBookContents, Encoding.UTF8, "text/plain");

                    var response = transactionNode.Node.Name switch
                    {
                        "Author.Api" => await _httpClientAuthorApi.PostAsync("commit", authorData),
                        "Book.Api" => await _httpClientBookApi.PostAsync("commit", bookData),
                    };

                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (responseContent == "OK")
                        transactionNode.TransactionState = TransactionState.Done;
                    else
                        transactionNode.TransactionState = TransactionState.Abort;
                }
                catch
                {
                    transactionNode.TransactionState = TransactionState.Abort;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task PrepareServices(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
               .Include(ns => ns.Node)
               .Where(n => n.TransactionId == transactionId)
               .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = transactionNode.Node.Name switch
                    {
                        "Author.Api" => await _httpClientAuthorApi.GetAsync("ready"),
                        "Book.Api" => await _httpClientBookApi.GetAsync("ready"),
                    };

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    await Console.Out.WriteLineAsync(result.ToString());
                    transactionNode.IsReady = result ? ReadyType.Ready : ReadyType.Unready;
                }
                catch
                {
                    transactionNode.IsReady = ReadyType.Unready;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task Rollback(Guid transactionId, CreateAuthorModel createAuthorModel)
        {
            var transactionNodes = await _context.NodeStates
               .Include(ns => ns.Node)
               .Where(ns => ns.TransactionId == transactionId)
               .ToListAsync();

            var jsonAuthorContent = JsonSerializer.Serialize(createAuthorModel.Author);
            var jsonBookContents = JsonSerializer.Serialize(createAuthorModel.Books);

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var authorData = new StringContent(jsonAuthorContent, Encoding.UTF8, "text/plain");
                    var bookData = new StringContent(jsonBookContents, Encoding.UTF8, "text/plain");

                    var response = transactionNode.Node.Name switch
                    {
                        "Author.Api" => await _httpClientAuthorApi.PostAsync("rollback", authorData),
                        "Book.Api" => await _httpClientBookApi.PostAsync("rollback", bookData),
                    };

                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (responseContent == "OK")
                        transactionNode.TransactionState = TransactionState.Done;
                    else
                        transactionNode.TransactionState = TransactionState.Abort;
                }
                catch
                {
                    transactionNode.TransactionState = TransactionState.Abort;
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
