using System;

namespace BatchPay.Data.Entities;

public sealed class GroupPaymentMemberEntity
{    public int Id { get; set; }

    // MODIFIED: Foreign keys are now generic
    public int GroupPaymentId { get; set; }
    public int MemberId { get; set; } // Renamed from UserId
    public bool IsActive { get; set; } = false;
    public GroupPaymentEntity GroupPayment { get; set; } = null!;
    public DirectoryEntryEntity Member { get; set; } = null!;
}