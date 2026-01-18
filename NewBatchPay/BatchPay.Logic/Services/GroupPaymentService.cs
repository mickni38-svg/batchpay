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

        var memberIds = dto.MemberUserIds.Distinct().ToList();
        if (!memberIds.Contains( dto.CreatedByUserId ))
            memberIds.Insert( 0, dto.CreatedByUserId );

        var count = await db.Users.CountAsync( u => memberIds.Contains( u.Id ), ct );
        if (count != memberIds.Count)
            throw new ArgumentException( "One or more users do not exist" );

        var entity = new GroupPaymentEntity
        {
            Title = title,
            Message = string.IsNullOrWhiteSpace( dto.Message ) ? null : dto.Message.Trim(),
            CreatedByUserId = dto.CreatedByUserId,
            IconKey = dto.IconKey,
            IsActive = dto.IsActive,
            CreatedAtUtc = DateTime.UtcNow,
            Members = memberIds.Select( id => new GroupPaymentMemberEntity { UserId = id } ).ToList()
        };

        db.GroupPayments.Add( entity );
        await db.SaveChangesAsync( ct );

        var members = await db.GroupPaymentMembers
            .Where( m => m.GroupPaymentId == entity.Id && EF.Property<bool>( m, "IsActive" ) )
            .Join( db.Users, m => m.UserId, u => u.Id, ( _, u ) => u )
            .OrderBy( u => u.DisplayName )
            .Select( u => new UserDto( u.Id, u.DisplayName, u.Handle ) )
            .ToListAsync( ct );

        return new GroupPaymentDto( entity.Id, entity.Title, entity.Message, entity.CreatedByUserId, entity.CreatedAtUtc, entity.IconKey, members );
    }

    public async Task<IReadOnlyList<GroupPaymentDto>> GetForUserAsync( int userId, CancellationToken ct )
    {
        // ✅ Kun aktive membership links
        var groupIds = await db.GroupPaymentMembers
            .Where( m => m.UserId == userId && EF.Property<bool>( m, "IsActive" ) )
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
            .Join( db.Users, m => m.UserId, u => u.Id,
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
}
