using BatchPay.Contracts.Dto;
using BatchPay.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Api.Controllers;

[ApiController]
[Route( "api/directory" )]
public sealed class DirectoryController( BatchPayContext db ) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DirectoryEntryDto>>> GetAll( CancellationToken ct )
    {
        var users = await db.Users
            .Select( u => new DirectoryEntryDto(
                DirectoryEntryType.User,
                u.Id,
                u.DisplayName,
                u.Handle,
                null,
                null
            ) )
            .ToListAsync( ct );

        var merchants = await db.Merchants
            .Where( m => m.IsActive )
            .Select( m => new DirectoryEntryDto(
                DirectoryEntryType.Merchant,
                m.Id,
                m.DisplayName,
                m.Handle,
                m.City,
                m.LogoUrl
            ) )
            .ToListAsync( ct );

        return Ok( users.Concat( merchants ).ToList() );
    }
}
