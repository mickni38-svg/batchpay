namespace BatchPay.Contracts.Dto;

/// <summary>
/// DTO for creating a new group payment.
/// </summary>
public sealed record CreateGroupPaymentRequestDto(
    string Title,
    string? Message,
    int CreatedByUserId,
    IReadOnlyList<int> MemberIds,
    string IconKey,
    bool IsActive);
