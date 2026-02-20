namespace BatchPay.Contracts.Dto;

public sealed record GroupPaymentMemberDto(
    int MemberId,
    string DisplayName,
    string Handle,
    byte MemberType);