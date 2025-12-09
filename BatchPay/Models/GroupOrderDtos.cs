namespace BatchPay.Models
{
    public sealed class CreateGroupOrderRequest
    {
        public int OwnerUserId { get; set; }
        public List<int> ParticipantUserIds { get; set; } = new();
        public string? Title { get; set; }
        public string? MerchantId { get; set; }
    }

    public sealed class CreateGroupOrderResponse
    {
        public int GroupOrderId { get; set; }
        public string GroupOrderCode { get; set; } = string.Empty;
    }
}
