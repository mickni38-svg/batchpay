using System;
using System.Collections.Generic;

namespace BatchPay.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string? OrderCode { get; set; }   // fx "ABC123"
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Status { get; set; }

        public int CreatedByUserId { get; set; }
        public User? CreatedBy { get; set; }

        public ICollection<OrderParticipant>? Participants { get; set; }
        public ICollection<OrderLine>? OrderLines { get; set; }
    }
}
