using BatchPay.Contracts.Dto;
using BatchPay.Data;
using BatchPay.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Logic.Services;

public sealed class FriendService( BatchPayContext db ) : IFriendService
{
    private const byte Accepted = 1;

    public async Task<IReadOnlyList<UserDto>> GetFriendsAsync( int requesterId, CancellationToken ct )
    {
        // This method is already correct.
        return await db.FriendRequests
            .Where( fr => fr.RequesterId == requesterId && fr.Status == Accepted )
            .Select( fr => fr.Receiver ) // Select the related DirectoryEntry (User or Merchant)
            .OrderBy( e => e.DisplayName )
            .Select( e => new UserDto( e.Id, e.DisplayName, e.Handle ) )
            .ToListAsync( ct );
    }

    public async Task<bool> AddFriendAsync( AddFriendRequestDto dto, CancellationToken ct )
    {
        // This code is now correct because it matches the updated DTO.
        if (dto.RequesterId == dto.ReceiverId)
        {
            return false;
        }

        // Check if both the requester and receiver exist in the directory.
        var requesterExists = await db.DirectoryEntries.AnyAsync( e => e.Id == dto.RequesterId, ct );
        var receiverExists = await db.DirectoryEntries.AnyAsync( e => e.Id == dto.ReceiverId, ct );

        if (!requesterExists || !receiverExists)
        {
            return false;
        }

        // Check if a friend request already exists.
        var exists = await db.FriendRequests.AnyAsync( fr =>
            fr.RequesterId == dto.RequesterId &&
            fr.ReceiverId == dto.ReceiverId, ct );

        if (exists)
        {
            return true; // Idempotent
        }

        db.FriendRequests.Add( new FriendRequestEntity
        {
            RequesterId = dto.RequesterId,
            ReceiverId = dto.ReceiverId,
            Status = Accepted,
            CreatedAt = DateTime.UtcNow
        } );

        await db.SaveChangesAsync( ct );
        return true;
    }
}
