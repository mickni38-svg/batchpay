namespace BatchPay.Models
{
    public class UserLite
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }

        public UserLite()
        {
            FullName = "";
            UserName = "";
            AvatarUrl = null;
        }
    }
}
