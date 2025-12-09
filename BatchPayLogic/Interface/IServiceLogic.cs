using BatchPay.ServiceLogic.Dtos;
using BatchPayServiceLogic.Dtos;
namespace BatchPay.ServiceLogic.Interface
{
    public interface IServiceLogic
    {

        public Task<List<UserLiteDto>> GetAllUsers( bool removeFriendsFromList = false );

        public Task<List<UserLiteDto>> TypeAheadSearch( string q );

        public Task<List<UserLiteDto>> GetMyFriends( int mySelf );
        Task<AddOrderFromMerchantResponse?> AddOrderFromMerchantAsync( AddOrderFromMerchantRequest request );

        Task<CreateGroupOrderResponse> CreateGroupOrderAsync( CreateGroupOrderRequest request );
    }
}
