namespace BatchPay.Contracts.Dto;

/// <summary>
/// IMPORTANT:
/// Request DTO når frontend opretter en gruppebetaling.
/// IconKey vælges af brugeren via ikon-picker.
/// </summary>
public sealed record CreateGroupPaymentRequestDto(
    string Title,
    string? Message,
    int CreatedByUserId,
    string IconKey,
    bool IsActive,
    DateTime CreatedAtUtc,
    IReadOnlyList<int> MemberUserIds
);
