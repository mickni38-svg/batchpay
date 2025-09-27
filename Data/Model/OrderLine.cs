
namespace BatchPay.Models
{
    public class OrderLine
    {
        public int OrderLineId { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public string? ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
