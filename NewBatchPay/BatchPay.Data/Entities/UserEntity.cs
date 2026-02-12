namespace BatchPay.Data.Entities;

public sealed class UserEntity
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = "";
    public string Handle { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
}
