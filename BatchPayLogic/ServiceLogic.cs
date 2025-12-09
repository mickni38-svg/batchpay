using BatchPay.ServiceLogic.Dtos;
using BatchPay.ServiceLogic.Interface;
using BatchPayServiceLogic.Dtos;
using Data;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.ServiceLogic
{
    public class ServiceLogic : IServiceLogic
    {
        private readonly BatchPayContext _db;

        public ServiceLogic( BatchPayContext db ) { _db = db; }


        public async Task<List<UserLiteDto>> GetAllUsers( bool removeFriendsFromList = false )
        {
            var userList = await _db.Users.Where( u => u.UserId != 13 ).ToListAsync();
            var filteredList = new List<User>();

            if (removeFriendsFromList == true)
            {
                var myfriends = GetMyFriends( 13 ).Result.Select( f => f.UserId ).ToList();
                userList = userList.Where( u => u.UserId != 13 && !myfriends.Contains( u.UserId ) ).ToList();
            }


            var mappedList = userList.Select( u => new UserLiteDto
            {
                UserName = u.Email,
                UserId = u.UserId,
                FullName = u.Name,
                AvatarUrl = u.AvatarUrl
            } ).ToList();

            return mappedList;
        }

        public async Task<List<UserLiteDto>> TypeAheadSearch( string q )
        {
            q = (q ?? "").Trim();

            var query = _db.Users.AsQueryable();
            if (q.Length >= 2)
            {
                query = query.Where( u =>
                    u.Name.Contains( q ) || u.Email.Contains( q ) );
            }

            var list = await query.Where( skipMigSelv => skipMigSelv.UserId == 13 )
                .Select( u => new UserLiteDto
                {
                    UserId = u.UserId,
                    FullName = u.Name,
                    UserName = u.Email,
                    AvatarUrl = u.AvatarUrl
                } )
              .ToListAsync();

            return list;
        }

        public async Task<List<UserLiteDto>> GetMyFriends( int mySelf )
        {
            var list = await _db.FriendRequests
              .Where( fr => fr.Status == "Accepted" &&
                          (fr.RequesterId == mySelf || fr.ReceiverId == mySelf) )
              .Select( fr => fr.RequesterId == mySelf ? fr.Receiver : fr.Requester )
              .Select( u => new UserLiteDto
              {
                  UserId = u.UserId,
                  FullName = u.Name,
                  UserName = u.Email,
                  AvatarUrl = u.AvatarUrl
              } )
              .ToListAsync();

            return list;
        }

        public async Task<CreateGroupOrderResponse> CreateGroupOrderAsync( CreateGroupOrderRequest request )
        {
            // TODO: Her skal du senere oprette rigtig GroupOrder i databasen.
            // Lige nu laver vi en dummy så frontend kan virke.

            await Task.CompletedTask;

            // Dummy: brug fx random kode – senere genererer du rigtigt id + kode fra DB
            var dummyId = 1;
            var dummyCode = "ABC123";

            return new CreateGroupOrderResponse
            {
                GroupOrderId = dummyId,
                GroupOrderCode = dummyCode
            };
        }

        public async Task<AddOrderFromMerchantResponse?> AddOrderFromMerchantAsync( AddOrderFromMerchantRequest request )
        {
            await Task.CompletedTask;

            return new AddOrderFromMerchantResponse
            {
                GroupOrderId = 42,
                ParticipantId = 1337,
                Status = "OrderAdded",
                Message = "Dummy implementation – replace with real logic.",
                AllParticipantsHaveOrders = false
            };
        }

    }
}
