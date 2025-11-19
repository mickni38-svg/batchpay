using BatchPay.Api.Dtos;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Api.Controllers
{
    [ApiController]
    [Route( "api/[controller]" )]
    public class FriendsController : ControllerBase
    {
        private readonly BatchPayContext _db;

        public FriendsController( BatchPayContext db )
        {
            _db = db;
        }

        private int GetCurrentUserId()
        {
            // Skift til rigtig auth senere
            var header = Request.Headers[ "X-UserId" ].FirstOrDefault();
            int id;
            if (!int.TryParse( header, out id )) id = 1; // fallback demo-bruger
            return id;
        }

        // GET /api/friends
        [HttpGet]
        public async Task<IActionResult> GetMyFriends()
        {
            var me = GetCurrentUserId();

            var list = await _db.FriendRequests
                .Where( fr => fr.Status == "Accepted" &&
                            (fr.RequesterId == me || fr.ReceiverId == me) )
                .Select( fr => fr.RequesterId == me ? fr.Receiver : fr.Requester )
                .Select( u => new UserLiteDto
                {
                    UserId = u.UserId,
                    FullName = u.Name,
                    UserName = u.Email,
                    AvatarUrl = u.AvatarUrl
                } )
                .ToListAsync();

            return Ok( list );
        }

        // POST /api/friends/{userId}
        [HttpPost( "{userId:int}" )]
        public async Task<IActionResult> AddFriend( int userId )
        {
            var me = GetCurrentUserId();
            if (me == userId) return BadRequest( "Kan ikke tilføje dig selv." );

            // findes en accepted relation allerede?
            var exists = await _db.FriendRequests.AnyAsync( fr =>
                fr.Status == "Accepted" &&
                ((fr.RequesterId == me && fr.ReceiverId == userId) ||
                 (fr.RequesterId == userId && fr.ReceiverId == me)) );

            if (!exists)
            {
                var row = new Data.Model.FriendRequest
                {
                    RequesterId = me,
                    ReceiverId = userId,
                    Status = "Accepted"
                };
                _db.FriendRequests.Add( row );
                await _db.SaveChangesAsync();
            }

            return NoContent();
        }

        // DELETE /api/friends/{userId}
        [HttpDelete( "{userId:int}" )]
        public async Task<IActionResult> RemoveFriend( int userId )
        {
            var me = GetCurrentUserId();

            var rel = await _db.FriendRequests.FirstOrDefaultAsync( fr =>
                fr.Status == "Accepted" &&
                ((fr.RequesterId == me && fr.ReceiverId == userId) ||
                 (fr.RequesterId == userId && fr.ReceiverId == me)) );

            if (rel == null) return NotFound();

            // vælg én af disse to strategier:
            //rel.Status = "Removed";
            //_db.Update(rel);

            _db.FriendRequests.Remove( rel ); // fysisk slet
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
