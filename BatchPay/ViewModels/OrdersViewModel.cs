using System.Collections.ObjectModel;
using BatchPay.Models;
using BatchPay.Stores;

namespace BatchPay.ViewModels;

public class OrdersViewModel
{
    private readonly GroupOrderStore _store;

    public ObservableCollection<GroupOrderSummary> Orders => _store.Orders;

    public OrdersViewModel( GroupOrderStore store )
    {
        _store = store;
    }
}
