namespace BatchPay.Models;

public sealed class GroupOrderSummary
{
    public int GroupOrderId { get; set; }
    public string GroupOrderCode { get; set; } = "";
    public string Title { get; set; } = "";
    public string MerchantName { get; set; } = "";
    public string Status { get; set; } = "Afventer"; // senere: I gang/Fuldført
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int ParticipantsCount { get; set; }
}
