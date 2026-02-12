using BatchPay.Frontend.Views;

namespace BatchPay.Frontend;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute( nameof( HomePage ), typeof( HomePage ) );
        Routing.RegisterRoute( nameof( OverviewPage ), typeof( OverviewPage ) );
        Routing.RegisterRoute( nameof( CreateGroupPaymentPage ), typeof( CreateGroupPaymentPage ) );
        Routing.RegisterRoute( nameof( FindPersonsPage ), typeof( FindPersonsPage ) );
        Routing.RegisterRoute( nameof( MessagesPage ), typeof( MessagesPage ) );
    }
}
