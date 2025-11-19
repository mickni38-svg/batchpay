using BatchPay.Pages;

namespace BatchPay;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // I Vievmodels refereres der til friends så derved linker hen på VennerPage
        Routing.RegisterRoute( "friends", typeof( VennerPage ) );
        Routing.RegisterRoute( "group-payment", typeof(GroupPaymentPage) );
        Routing.RegisterRoute( "notifications", typeof( NotificationsPage ) );
    }
}
