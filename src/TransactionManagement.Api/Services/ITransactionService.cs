
using TransactionManagement.Data;

namespace TransactionManagement.Api.Services
{
    public interface ITransactionService
    {
        public Task<Guid> CreateTransactionAsync(Guid transactionId, params string[] names);
        public Task<bool> CheckReadyServices(Guid transactionId);
        public Task<bool> CheckTransactionStateServices(Guid transactionId);
    }
}
