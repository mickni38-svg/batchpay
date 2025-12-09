using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchPay.Models
{
    public class FriendSelectionItem
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;

        // Bindes til CheckBox
        public bool IsSelected { get; set; }
    }
}
