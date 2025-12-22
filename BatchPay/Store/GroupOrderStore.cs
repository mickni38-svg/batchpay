using System.Collections.ObjectModel;
using BatchPay.Models;

namespace BatchPay.Stores;

public sealed class GroupOrderStore
{
    public ObservableCollection<GroupOrderSummary> Orders { get; } = new();

    public void Add( GroupOrderSummary order )
    {
        // Insert øverst
        Orders.Insert( 0, order );
    }
}
