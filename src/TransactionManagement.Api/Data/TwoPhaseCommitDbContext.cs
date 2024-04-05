using Microsoft.EntityFrameworkCore;
using TransactionManagement.Entities;

namespace TransactionManagement.Data
{
    public class TwoPhaseCommitDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Node> Nodes { get; set; }
        public DbSet<NodeState> NodeStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Node>().HasData(
                    new Node("Author.Api") { Id = Guid.NewGuid() },
                    new Node("Book.Api") { Id = Guid.NewGuid() }
                );
        }
    }
}
