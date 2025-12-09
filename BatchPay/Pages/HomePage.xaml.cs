using BatchPay.ViewModels;

namespace BatchPay.Pages;

public partial class HomePage : ContentPage
{
    private readonly GroupPaymentPage _groupPaymentPage;

    public HomePage( HomeViewModel vm, GroupPaymentPage groupPaymentPage )
    {
        InitializeComponent();
        BindingContext = vm;
        _groupPaymentPage = groupPaymentPage;
    }

    private async void OnGroupPaymentClicked( object sender, EventArgs e )
    {
        await Navigation.PushAsync( _groupPaymentPage );
    }
}
