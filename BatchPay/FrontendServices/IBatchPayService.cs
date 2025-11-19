using BatchPay.Models;  // <- vigtigt

namespace FrontendServices
{
    public interface IBatchPayService
    {
        Task<IReadOnlyList<UserLite>> GetAllUsersAsync();
        Task<IReadOnlyList<UserLite>> SearchUsersAsync( string q );
        Task<IReadOnlyList<UserLite>> GetFriendsAsync();
        Task<bool> AddFriendAsync( int userId );
        Task<bool> RemoveFriendAsync( int userId );
    }
}
