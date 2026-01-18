namespace BatchPay.Data.Entities;

public sealed class GroupPaymentMemberEntity
{
    public int Id { get; set; }
    public int GroupPaymentId { get; set; }
    public int UserId { get; set; }
    public bool IsActive { get; set; } = true;
    public GroupPaymentEntity? GroupPayment { get; set; }
    public UserEntity? User { get; set; }
}
