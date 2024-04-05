using Author.Api.Data;

namespace Author.Api.Seeds
{
    public static class AuthorSeedContext
    {
        public static async Task SeedAsync(AuthorDbContext context)
        {
            if (!context.Set<Author.Api.Entities.Author>().Any())
            {
                var authors = new List<Author.Api.Entities.Author>()
                {
                    new(){Id = 1, Name = "Thomas man",CreatedDate = DateTime.UtcNow},
                    new(){Id = 2, Name = "Mark Twain",CreatedDate = DateTime.UtcNow},
                    new(){Id = 3, Name = "Ahmet Hasim.",CreatedDate = DateTime.UtcNow},
                };

                await context.Set<Author.Api.Entities.Author>().AddRangeAsync(authors);
                await context.SaveChangesAsync();
            }
        }
    }
}
