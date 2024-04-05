using TransactionManagement.Api.Entities;

namespace TransactionManagement.Api.Model
{
    public class CreateAuthorModel
    {
        public Author Author { get; set; }
        public List<Book> Books { get; set; }
    }
}
