using BatchPay.ViewModels;

namespace BatchPay.Pages;

public partial class HomePage : ContentPage
{
    private readonly GroupPaymentPage _groupPaymentPage;
    private readonly OrdersPage _ordersPage;
    private readonly GroupPaymentCreatePage _createPage;

    public HomePage( HomeViewModel vm, GroupPaymentPage groupPaymentPage, OrdersPage ordersPage, GroupPaymentCreatePage createPage )
    {
        InitializeComponent();
        BindingContext = vm;
        _groupPaymentPage = groupPaymentPage;
        _ordersPage = ordersPage;
        _createPage = createPage;
    }

    private async void OnGroupPaymentClicked( object sender, EventArgs e )
    {
        await Navigation.PushAsync( _groupPaymentPage );
    }

    private async void OnOrdersClicked( object sender, EventArgs e )
    {
        await Navigation.PushAsync( _ordersPage );
    }

    private async void OnCreateGroupPaymentClicked( object sender, EventArgs e )
    {
        await Shell.Current.GoToAsync( "group-payment-create" );
    }

}
