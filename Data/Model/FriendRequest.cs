using System;

namespace Data.Model
{
    public class FriendRequest
    {
        public int FriendRequestId { get; set; }

        public int RequesterId { get; set; }   // FK -> Users(UserId)
        public int ReceiverId { get; set; }   // FK -> Users(UserId)

        public string Status { get; set; }     // "Pending"|"Accepted"|"Rejected"|"Removed"
        public DateTime CreatedAtUtc { get; set; }

        // Navigationer
        public User Requester { get; set; }
        public User Receiver { get; set; }

        public FriendRequest()
        {
            Status = "Pending";
           // CreatedAtUtc = DateTime.UtcNow;
        }
    }
}
