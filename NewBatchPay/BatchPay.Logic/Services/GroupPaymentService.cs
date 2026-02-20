using BatchPay.Contracts.Dto;
using BatchPay.Data;
using BatchPay.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Logic.Services;

public sealed class GroupPaymentService( BatchPayContext db ) : IGroupPaymentService
{
    public async Task<GroupPaymentDto> CreateAsync( CreateGroupPaymentRequestDto dto, CancellationToken ct )
    {
        var title = (dto.Title ?? "").Trim();
        if (title.Length == 0) throw new ArgumentException( "Title is required" );

        // MODIFIED: Use the new MemberIds property from the DTO
        var memberIds = dto.MemberIds.Distinct().ToList();
        if (!memberIds.Contains( dto.CreatedByUserId ))
        {
            memberIds.Insert( 0, dto.CreatedByUserId );
        }

        var count = await db.DirectoryEntries.CountAsync( u => memberIds.Contains( u.Id ), ct );
        if (count != memberIds.Count)
            throw new ArgumentException( "One or more users do not exist" );

        var entity = new GroupPaymentEntity
        {
            Title = title,
            Message = string.IsNullOrWhiteSpace( dto.Message ) ? null : dto.Message.Trim(),
            CreatedByUserId = dto.CreatedByUserId,
            IconKey = dto.IconKey ?? string.Empty,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            Members = memberIds.Select( id => new GroupPaymentMemberEntity { Id = id } ).ToList()
        };

        db.GroupPayments.Add( entity );
        await db.SaveChangesAsync( ct );

        var members = await db.GroupPaymentMembers
            .Where( m => m.GroupPaymentId == entity.Id && EF.Property<bool>( m, "IsActive" ) )
            .Join( db.DirectoryEntries, m => m.Id, u => u.Id, ( _, u ) => u )
            .OrderBy( u => u.DisplayName )
            .Select( u => new UserDto( u.Id, u.DisplayName, u.Handle ) )
            .ToListAsync( ct );

        return new GroupPaymentDto( entity.Id, entity.Title, entity.Message, entity.CreatedByUserId, entity.CreatedAtUtc, entity.IconKey ?? string.Empty, members );
    }

    public async Task<IReadOnlyList<GroupPaymentDto>> GetForUserAsync( int userId, CancellationToken ct )
    {
        // ✅ Kun aktive membership links
        var groupIds = await db.GroupPaymentMembers
            .Where( m => m.Id == userId && EF.Property<bool>( m, "IsActive" ) )
            .Select( m => m.GroupPaymentId )
            .Distinct()
            .ToListAsync( ct );

        if (groupIds.Count == 0)
            return [];

        // ✅ Kun aktive gruppebetalinger
        var groups = await db.GroupPayments
            .Where( g => groupIds.Contains( g.Id ) && EF.Property<bool>( g, "IsActive" ) )
            .OrderByDescending( g => g.CreatedAtUtc )
            .ToListAsync( ct );

        // ✅ Kun aktive membership links
        var members = await db.GroupPaymentMembers
            .Where( m => groupIds.Contains( m.GroupPaymentId ) && EF.Property<bool>( m, "IsActive" ) )
            .Join( db.DirectoryEntries, m => m.Id, u => u.Id,
                ( m, u ) => new { m.GroupPaymentId, User = new UserDto( u.Id, u.DisplayName, u.Handle ) } )
            .ToListAsync( ct );

        return groups.Select( g =>
            new GroupPaymentDto(
                g.Id,
                g.Title,
                g.Message,
                g.CreatedByUserId,
                g.CreatedAtUtc,
                g.IconKey,
                members.Where( x => x.GroupPaymentId == g.Id ).Select( x => x.User ).ToList()
            )
        ).ToList();
    }

    public async Task<IReadOnlyList<GroupPaymentMemberDto>> GetMembersAsync( int groupPaymentId, CancellationToken ct )
    {
        // Since GroupPaymentMemberEntity does not have MemberType, we need to join on the correct key.
        // Assuming MemberId is the foreign key to DirectoryEntryEntity or Merchant.
        // We'll need to distinguish between user and merchant members by checking the type of the member.
        // For now, let's assume all members are users (DirectoryEntryEntity). If you have a way to distinguish merchants, update accordingly.

        var userMembers = db.GroupPaymentMembers
            .Where( gpm => gpm.GroupPaymentId == groupPaymentId )
            .Join( db.DirectoryEntries,
                  gpm => gpm.MemberId,
                  u => u.Id,
                  ( gpm, u ) => new
                  {
                      MemberId = u.Id,
                      DisplayName = u.DisplayName,
                      Handle = u.Handle,
                      MemberType = (byte)0 // 0 for user
                  } );

        // If you have merchant members, add similar logic here for merchants.

        return await userMembers
            .OrderBy( x => x.DisplayName )
            .Select( x => new GroupPaymentMemberDto( x.MemberId, x.DisplayName, x.Handle, x.MemberType ) )
            .ToListAsync( ct );
    }
}
