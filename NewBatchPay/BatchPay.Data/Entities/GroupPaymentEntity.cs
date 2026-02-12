namespace BatchPay.Data.Entities;

public sealed class GroupPaymentEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Message { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? IconKey { get; set; }
    public bool IsActive { get; set; }
    public UserEntity? CreatedBy { get; set; }
    public List<GroupPaymentMemberEntity> Members { get; set; } = new();
}
