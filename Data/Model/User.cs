using System.Collections.Generic;

namespace BatchPay.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public ICollection<OrderParticipant>? OrderParticipants { get; set; }
        public ICollection<OrderLine>? OrderLines { get; set; }
    }
}
