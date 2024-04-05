using Author.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using TransactionManagement.Api.Model;

namespace Author.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController(AuthorDbContext _context, HttpClient httpClient, IHttpClientFactory _httpClientFactory) : ControllerBase
    {
        HttpClient _httpClientTransactionApi = _httpClientFactory.CreateClient("TransactionManagementApi");

        [HttpGet("GetBooksToAuthor")]
        public async Task<IActionResult> GetBooksToAuthor([FromQuery] int authorId)
        {
            var author = await _context.Set<Author.Api.Entities.Author>().FirstOrDefaultAsync(a => a.Id == authorId);

            var response = await httpClient.GetStringAsync($"https://localhost:7179/api/book/GetBooksToId?authorId={authorId}");
            if (response is null)
                return BadRequest();
            var bookModel = JsonSerializer.Deserialize<Book.Api.Models.Book>(response);
            return Ok(bookModel);
        }

        [HttpPost("CreateAuthorWithBooks")]
        public async Task<IActionResult> CreateAuthorWithBooks([FromBody] CreateAuthorModel createAuthorModel)
        {
            _httpClientTransactionApi.Timeout = TimeSpan.FromSeconds(200);

            var data = JsonSerializer.Serialize(createAuthorModel);
            var authorData = new StringContent(data, Encoding.UTF8, "text/plain");

            var response = await _httpClientTransactionApi.PostAsync("create-author-transaction", authorData);
            var jsonString = await response.Content.ReadAsStringAsync();
            bool reply = JsonSerializer.Deserialize<bool>(jsonString);

            return reply is true ? Ok() : BadRequest();
        }

        [HttpGet("Get-All-Authors")]
        public async Task<IActionResult> GetAllAuthors()
        {
            return Ok(await _context.Authors.ToListAsync());
        }
    }
}
