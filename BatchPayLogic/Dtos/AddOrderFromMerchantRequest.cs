namespace BatchPayServiceLogic.Dtos;

public sealed class AddOrderFromMerchantRequest
{
    public string MerchantId { get; set; } = default!;
    public string UserExternalId { get; set; } = default!;
    public string GroupOrderCode { get; set; } = default!;
    public string? OrderReference { get; set; }
    public string Currency { get; set; } = "DKK";
    public decimal TotalAmount { get; set; }
    public List<MerchantOrderLineDto> OrderLines { get; set; } = new();
    public string? PaymentTransactionId { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

public sealed class MerchantOrderLineDto
{
    public string? Sku { get; set; }
    public string Name { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public sealed class AddOrderFromMerchantResponse
{
    public int GroupOrderId { get; set; }
    public int ParticipantId { get; set; }
    public string Status { get; set; } = default!;
    public string? Message { get; set; }
    public bool AllParticipantsHaveOrders { get; set; }
}
