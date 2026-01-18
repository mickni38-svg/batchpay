namespace BatchPay.Data.Entities;

public sealed class MerchantEntity
{
    public int Id { get; set; }

    // Visning / identitet
    public string DisplayName { get; set; } = "";
    public string Handle { get; set; } = ""; // fx "@pizzapalace"
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }

    // Kontakt (B2B)
    public string ContactEmail { get; set; } = "";
    public string? ContactPhone { get; set; }

    // Adresse (valgfrit)
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? CountryCode { get; set; } // "DK"

    // Kommercielt / adgang
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }

    // Navigation
    public MerchantIntegrationEntity? Integration { get; set; }
}
