using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BatchPay.Data;
using System.Linq;

namespace BatchPay.Api.Controllers
{
    [ApiController]
    [Route( "api/[controller]" )]
    public class OrdersController : ControllerBase
    {
        private readonly BatchPayContext _context;

        public OrdersController( BatchPayContext context )
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include( o => o.OrderLines )
                .Include( o => o.Participants )
                    .ThenInclude( p => p.User )
                .Select( o => new
                {
                    o.OrderId,
                    o.Title,
                    o.Status,
                    OrderLines = o.OrderLines.Select( l => new
                    {
                        l.ItemName,
                        l.Quantity,
                        l.Price
                    } ),
                    Participants = o.Participants.Select( p => new
                    {
                        UserName = p.User.Name,
                        p.HasPaid
                    } )
                } )
                .ToListAsync();

            return Ok( orders );
        }
    }
}
