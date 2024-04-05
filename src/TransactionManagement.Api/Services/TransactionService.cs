using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using TransactionManagement.Data;
using TransactionManagement.Entities;
using TransactionManagement.Enums;

namespace TransactionManagement.Api.Services
{
    public class TransactionService(IHttpClientFactory _httpClientFactory, TwoPhaseCommitDbContext _context) : ITransactionService
    {
        public async Task<bool> CheckReadyServices(Guid transactionId) =>
            (await _context.NodeStates
                    .Where(ns => ns.TransactionId == transactionId)
                    .ToListAsync())
            .TrueForAll(n => n.IsReady == ReadyType.Ready);

        public async Task<bool> CheckTransactionStateServices(Guid transactionId) =>
            (await _context.NodeStates
                    .Where(ns => ns.TransactionId == transactionId)
                    .ToListAsync())
            .TrueForAll(n => n.TransactionState == TransactionState.Done);

        public async Task<Guid> CreateTransactionAsync(Guid transactionId, params string[] names)
        {
            foreach (var name in names)
            {
                List<Node>? nodes = await _context.Nodes.Where(n => n.Name == name).ToListAsync();
                nodes.ForEach(node => node.NodeStates = new List<NodeState>
                {
                    new(TransactionId : transactionId)
                    {
                        IsReady = ReadyType.Pending,
                        TransactionState = TransactionState.Pending
                    },
                });
            }

            await _context.SaveChangesAsync();
            return transactionId;
        }
    }
}
