using System.Collections.Generic;

namespace Data.Model
{
    public class User
    {
        public int UserId { get; set; }      // PK
        public string Name { get; set; }     // not null
        public string Email { get; set; }    // not null
        public string Phone { get; set; }    // null
        public string AvatarUrl { get; set; }// null

        // Navigationer (valgfri men anbefalet)
        public ICollection<FriendRequest> SentFriendRequests { get; set; }
        public ICollection<FriendRequest> ReceivedFriendRequests { get; set; }

        public User()
        {
            Name = "";
            Email = "";
            SentFriendRequests = new List<FriendRequest>();
            ReceivedFriendRequests = new List<FriendRequest>();
        }
    }
}
