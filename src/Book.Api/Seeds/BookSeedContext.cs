using Book.Api.Data;

namespace Book.Api.Seeds
{
    public static class BookSeedContext
    {
        public static async Task SeedAsync(BookDbContext context)
        {
            if (!context.Set<Book.Api.Entities.Book>().Any())
            {
                var Books = new List<Book.Api.Entities.Book>()
                {
                   new(){Id = 1,AuthorId = 1,Title = "Thomas Man TITLE"},
                   new(){Id = 2,AuthorId = 2,Title = "Mark Twain TITLE"},
                   new(){Id = 3,AuthorId = 3,Title = "Ahmet Hasim TITLE"}
                };

                await context.Set<Book.Api.Entities.Book>().AddRangeAsync(Books);
                await context.SaveChangesAsync();
            }
        }
    }
}
