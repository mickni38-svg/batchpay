using BatchPay.Contracts.Dto;
namespace BatchPay.Logic.Services;

public interface IFriendService
{
    Task<IReadOnlyList<UserDto>> GetFriendsAsync( int requesterUserId, CancellationToken ct );
    Task<bool> AddFriendAsync( AddFriendRequestDto dto, CancellationToken ct );
}
