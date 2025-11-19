using BatchPay.Api.Dtos;
using Data;
using Data.Model;
using Microsoft.EntityFrameworkCore;
namespace BatchPayLogic
{
     public interface IServiceLogic
    {

        public  Task<List<UserLiteDto>> GetAllUsers( bool removeFriendsFromList = false );

        public  Task<List<UserLiteDto>> TypeAheadSearch( string q );

        public Task<List<UserLiteDto>> GetMyFriends( int mySelf );
    }
}
