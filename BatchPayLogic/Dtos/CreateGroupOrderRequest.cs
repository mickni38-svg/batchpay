namespace BatchPayServiceLogic.Dtos;

public sealed class CreateGroupOrderRequest
{
    /// <summary>Id på brugeren der opretter gruppeordren.</summary>
    public int OwnerUserId { get; set; }

    /// <summary>De venner der skal inviteres (UserId fra din backend).</summary>
    public List<int> ParticipantUserIds { get; set; } = new();

    /// <summary>Valgfri tekst f.eks. "Tony's pizza fredag aften".</summary>
    public string? Title { get; set; }

    /// <summary>Valgfrit – senere kan du sætte MerchantId her.</summary>
    public string? MerchantId { get; set; }
}

public sealed class CreateGroupOrderResponse
{
    public int GroupOrderId { get; set; }

    /// <summary>Kort kode som kan bruges på MadSted eller deles med venner.</summary>
    public string GroupOrderCode { get; set; } = default!;
}
