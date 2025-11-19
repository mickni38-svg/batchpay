using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace BatchPay.ViewModels;

public partial class HomeViewModel
{
    public string FooterText => "POC: Navigation via ViewModel med dummydata";

    [RelayCommand]
    private Task GoToFriendsAsync()
        => Shell.Current.GoToAsync( "friends" );

    [RelayCommand]
    private Task GoToGroupPaymentAsync()
        => Shell.Current.GoToAsync( "group-payment" );

    [RelayCommand]
    private Task GoToNotificationsAsync()
        => Shell.Current.GoToAsync( "notifications" );
}
