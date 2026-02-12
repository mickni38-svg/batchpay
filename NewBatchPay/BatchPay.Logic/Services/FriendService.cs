using BatchPay.Contracts.Dto;
using BatchPay.Data;
using BatchPay.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Logic.Services;

public sealed class FriendService( BatchPayContext db ) : IFriendService
{
    private const byte Accepted = 1;

    public async Task<IReadOnlyList<UserDto>> GetFriendsAsync( int requesterUserId, CancellationToken ct )
    {
        // Venner = Requester -> Receiver
        return await db.FriendRequests
    .Where( fr => fr.RequesterUserId == requesterUserId && fr.Status == Accepted )
    .Join( db.Users, fr => fr.ReceiverUserId, u => u.Id, ( fr, u ) => u )
    .OrderBy( u => u.DisplayName )
    .Select( u => new UserDto( u.Id, u.DisplayName, u.Handle ) )
    .ToListAsync( ct );
    }

    public async Task<bool> AddFriendAsync( AddFriendRequestDto dto, CancellationToken ct )
    {
        if (dto.RequesterUserId == dto.ReceiverUserId) return false;

        var requesterExists = await db.Users.AnyAsync( x => x.Id == dto.RequesterUserId, ct );
        var receiverExists = await db.Users.AnyAsync( x => x.Id == dto.ReceiverUserId, ct );
        if (!requesterExists || !receiverExists) return false;

        var exists = await db.FriendRequests.AnyAsync( x =>
            x.RequesterUserId == dto.RequesterUserId &&
            x.ReceiverUserId == dto.ReceiverUserId, ct );

        if (exists) return true; // idempotent

        db.FriendRequests.Add( new FriendRequestEntity
        {
            RequesterUserId = dto.RequesterUserId,
            ReceiverUserId = dto.ReceiverUserId,
            Status = Accepted
        } );

        await db.SaveChangesAsync( ct );
        return true;
    }
}
