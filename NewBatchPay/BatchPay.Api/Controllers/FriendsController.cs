using BatchPay.Contracts.Dto;
using BatchPay.Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace BatchPay.Api.Controllers;

[ApiController]
[Route( "api/friends" )]
public sealed class FriendsController( IFriendService friends ) : ControllerBase
{
    [HttpGet( "{requesterUserId:int}" )]
    public async Task<IActionResult> GetFriends( int requesterUserId, CancellationToken ct )
        => Ok( await friends.GetFriendsAsync( requesterUserId, ct ) );

    [HttpPost]
    public async Task<IActionResult> AddFriend( [FromBody] AddFriendRequestDto dto, CancellationToken ct )
        => (await friends.AddFriendAsync( dto, ct )) ? Ok() : BadRequest();
}
