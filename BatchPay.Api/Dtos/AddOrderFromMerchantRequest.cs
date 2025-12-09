namespace BatchPay.Api.Dtos
{
    public sealed class AddOrderFromMerchantRequest
    {
        public string MerchantId { get; set; } = default!;
        public string UserExternalId { get; set; } = default!;

        /// <summary>
        /// Kode der binder MadSted ordre til en eksisterende gruppeordre i BatchPay.
        /// </summary>
        public string GroupOrderCode { get; set; } = default!;

        /// <summary>
        /// MadSteds egen ordre-reference (til kassesystem/webshop).
        /// </summary>
        public string? OrderReference { get; set; }

        public string Currency { get; set; } = "DKK";

        /// <summary>
        /// Beløb for denne persons bestilling (inkl. moms).
        /// </summary>
        public decimal TotalAmount { get; set; }

        public List<MerchantOrderLineDto> OrderLines { get; set; } = new();

        /// <summary>
        /// Reference til en reservation/transaction hos Nets eller anden PSP.
        /// </summary>
        public string? PaymentTransactionId { get; set; }

        /// <summary>
        /// Fritekst/fri JSON fra merchant (bordnr, note, kampagnekode, osv.).
        /// </summary>
        public Dictionary<string, string>? Metadata { get; set; }
    }

    public sealed class MerchantOrderLineDto
    {
        /// <summary>
        /// Merchantens interne varenummer eller sku.
        /// </summary>
        public string? Sku { get; set; }

        public string Name { get; set; } = default!;

        public int Quantity { get; set; }

        /// <summary>
        /// Pris pr. enhed (inkl. moms).
        /// </summary>
        public decimal UnitPrice { get; set; }
    }

    public sealed class AddOrderFromMerchantResponse
    {
        public int GroupOrderId { get; set; }
        public int ParticipantId { get; set; }

        /// <summary>
        /// Fx: "OrderAdded", "OrderUpdated", "GroupOrderCompleted"
        /// </summary>
        public string Status { get; set; } = default!;

        public string? Message { get; set; }

        /// <summary>
        /// True hvis alle deltagere nu har en ordre knyttet til sig.
        /// </summary>
        public bool AllParticipantsHaveOrders { get; set; }
    }
}
