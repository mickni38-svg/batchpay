
namespace BatchPay.Models
{
    public class OrderParticipant
    {
        public int OrderParticipantId { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public bool IsOwner { get; set; }
        public bool HasPaid { get; set; }
    }
}
