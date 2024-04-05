using System.ComponentModel.DataAnnotations.Schema;
using TransactionManagement.Enums;

namespace TransactionManagement.Entities
{
    public record NodeState(Guid TransactionId)
    {
        public Guid Id { get; set; }
        public ReadyType IsReady { get; set; }
        public TransactionState TransactionState { get; set; }

        [ForeignKey("NodeId")]
        public Guid NodeId { get; set; }
        public Node Node { get; set; }
    }
}
