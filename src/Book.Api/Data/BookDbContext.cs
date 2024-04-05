using Microsoft.EntityFrameworkCore;

namespace Book.Api.Data
{
    public class BookDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Book.Api.Entities.Book> Books { get; set; }
    }
}
