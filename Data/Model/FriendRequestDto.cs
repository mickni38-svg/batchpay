using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchPay.Api.Dto
{
    public class FriendRequestDto
    {
        public int RequesterId { get; set; }
        public int ReceiverId { get; set; }
    }
}
