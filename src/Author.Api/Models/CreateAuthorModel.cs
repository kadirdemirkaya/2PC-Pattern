namespace TransactionManagement.Api.Model
{
    public class CreateAuthorModel
    {
        public Author.Api.Entities.Author Author { get; set; }
        public List<Book.Api.Models.Book> Books { get; set; }
    }
}
