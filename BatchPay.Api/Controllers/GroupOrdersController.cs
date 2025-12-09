using BatchPay.ServiceLogic.Interface;
using BatchPayServiceLogic.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BatchPay.Api.Controllers;

[ApiController]
[Route( "api/[controller]" )]
public sealed class GroupOrdersController : ControllerBase
{
    private readonly IServiceLogic _serviceLogic;

    public GroupOrdersController( IServiceLogic serviceLogic )
    {
        _serviceLogic = serviceLogic;
    }

    [HttpPost( "create" )]
    public async Task<ActionResult<CreateGroupOrderResponse>> Create( [FromBody] CreateGroupOrderRequest request )
    {
        if (!ModelState.IsValid)
            return BadRequest( ModelState );

        var result = await _serviceLogic.CreateGroupOrderAsync( request );
        return Ok( result );
    }

    [HttpPost( "add-order-from-merchant" )]
    public async Task<ActionResult<AddOrderFromMerchantResponse>> AddOrderFromMerchant(
        [FromBody] AddOrderFromMerchantRequest request )
    {
        if (!ModelState.IsValid)
            return BadRequest( ModelState );

        var result = await _serviceLogic.AddOrderFromMerchantAsync( request );

        if (result is null)
        {
            return NotFound( new
            {
                errorCode = "GroupOrderNotFound",
                message = "No active group order found matching the request."
            } );
        }

        return Ok( result );
    }
}
