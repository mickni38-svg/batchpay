using BatchPay.Contracts.Dto;
using BatchPay.Contracts.Icons;
using BatchPay.Data;
using BatchPay.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Backend.Controllers;

[ApiController]
[Route( "api/group-payments" )]
public sealed class GroupPaymentsController : ControllerBase
{
    private readonly IGroupPaymentService _service;
    private readonly BatchPayContext _db;

    public GroupPaymentsController( IGroupPaymentService service, BatchPayContext db )
    {
        _service = service;
        _db = db;
    }

    /// <summary>
    /// Opret gruppebetaling med IconKey valgt af brugeren.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<GroupPaymentDto>> CreateAsync(
        [FromBody] CreateGroupPaymentRequestDto request,
        CancellationToken ct )
    {
        if (string.IsNullOrWhiteSpace( request.Title ))
            return BadRequest( "Title is required." );

        if (!IconCatalog.IsValid( request.IconKey ))
            return BadRequest( "Invalid IconKey." );

        var created = await _service.CreateAsync( request, ct );
        return Ok( created );
    }

    /// <summary>
    /// Hent gruppebetalinger for user (kun aktive).
    /// </summary>
    [HttpGet( "for-user/{userId:int}" )]
    public async Task<ActionResult<IReadOnlyList<GroupPaymentDto>>> GetForUserAsync(
        int userId,
        CancellationToken ct )
    {
        var list = await _service.GetForUserAsync( userId, ct );
        return Ok( list );
    }

    /// <summary>
    /// ✅ Soft delete (deaktiverer) en gruppebetaling og dens membership-rækker.
    /// </summary>
    [HttpDelete( "{groupPaymentId:int}" )]
    public async Task<IActionResult> DeactivateAsync( int groupPaymentId, CancellationToken ct )
    {
        var gp = await _db.GroupPayments.FirstOrDefaultAsync( x => x.Id == groupPaymentId, ct );
        if (gp is null)
            return NotFound();

        // Hvis den allerede er deaktiveret
        var isActive = _db.Entry( gp ).Property<bool>( "IsActive" ).CurrentValue;
        if (!isActive)
            return NoContent();

        var now = DateTime.UtcNow;

        _db.Entry( gp ).Property<bool>( "IsActive" ).CurrentValue = false;
        _db.Entry( gp ).Property<DateTime?>( "DeactivatedAtUtc" ).CurrentValue = now;

        var links = await _db.GroupPaymentMembers
            .Where( m => m.GroupPaymentId == groupPaymentId )
            .ToListAsync( ct );

        foreach (var m in links)
        {
            _db.Entry( m ).Property<bool>( "IsActive" ).CurrentValue = false;
            _db.Entry( m ).Property<DateTime?>( "DeactivatedAtUtc" ).CurrentValue = now;
        }

        await _db.SaveChangesAsync( ct );
        return NoContent();
    }
}