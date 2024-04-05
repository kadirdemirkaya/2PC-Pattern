using TransactionManagement.Api.Model;

namespace TransactionManagement.Api.Services.Author
{
    public interface IAuthorTransactionService : ITransactionService
    {
        public Task PrepareServices(Guid transactionId);
        public Task Commit(Guid transactionId, CreateAuthorModel createAuthorModel);
        public Task Rollback(Guid transactionId, CreateAuthorModel createAuthorModel);
    }
}
