using BatchPay.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BatchPay.Pages;

public partial class OrdersPage : ContentPage
{
    public OrdersPage()
    {
        InitializeComponent();

        // Hent VM fra DI (Shell kan oprette parameterløse Pages)
        BindingContext = IPlatformApplication.Current.Services.GetRequiredService<OrdersViewModel>();
    }
}
