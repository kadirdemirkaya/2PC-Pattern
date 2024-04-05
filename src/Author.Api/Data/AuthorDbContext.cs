using Microsoft.EntityFrameworkCore;

namespace Author.Api.Data
{
    public class AuthorDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Author.Api.Entities.Author> Authors { get; set; }
    }
}
