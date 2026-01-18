namespace BatchPay.Data.Entities;

public sealed class FriendRequestEntity
{
    public int Id { get; set; }
    public int RequesterUserId { get; set; }
    public int ReceiverUserId { get; set; }
    public byte Status { get; set; } // 1=Accepted, 0=Pending, 2=Declined
    public DateTime CreatedAtUtc { get; set; }

    public UserEntity? Requester { get; set; }
    public UserEntity? Receiver { get; set; }
}
