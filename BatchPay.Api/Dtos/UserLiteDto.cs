namespace BatchPay.Api.Dtos
{
    public class UserLiteDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }  // map fra Users.Name
        public string UserName { get; set; }  // map fra Users.Email (eller brugernavn)
        public string AvatarUrl { get; set; }
        public bool IsFriend { get; set; }
    }
}
