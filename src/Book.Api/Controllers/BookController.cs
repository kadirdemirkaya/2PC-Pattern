using Book.Api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController(BookDbContext _context) : ControllerBase
    {
        [HttpGet("GetBooksToId")]
        public async Task<IActionResult> GetBooksToId([FromQuery] int authorId)
        {
            var response = await _context.Set<Book.Api.Entities.Book>().FirstOrDefaultAsync(b => b.AuthorId == authorId);
            return Ok(response);
        }
    }
}
