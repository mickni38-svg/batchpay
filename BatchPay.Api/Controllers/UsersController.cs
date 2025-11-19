using BatchPay.Api.Dtos;
using BatchPayLogic;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Api.Controllers
{
    [ApiController]
    [Route( "api/[controller]" )]
    public class UsersController : ControllerBase
    {
        private IServiceLogic _logic;
        public UsersController( IServiceLogic logic ) { _logic = logic; }

        // GET /api/users  (global liste)
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            bool filterOutFriends = true;
           var list = await _logic.GetAllUsers( filterOutFriends );

            return Ok( list );
        }

        // GET /api/users/search?q=...
        [HttpGet( "search" )]
        public async Task<IActionResult> Search( [FromQuery] string q )
        {

            var list = await _logic.TypeAheadSearch( q );
            
            return Ok( list );
        }
    }
}
