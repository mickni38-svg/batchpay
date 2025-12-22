using BatchPay.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BatchPay.Pages;

public partial class GroupPaymentCreatePage : ContentPage
{
    private readonly GroupPaymentCreateViewModel _vm;

    public GroupPaymentCreatePage()
    {
        InitializeComponent();

        _vm = IPlatformApplication.Current.Services.GetRequiredService<GroupPaymentCreateViewModel>();
        BindingContext = _vm;

        // Subscribe én gang i constructor (ikke i OnAppearing)
        _vm.GroupOrderCreated += OnGroupOrderCreated;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadCommand.Execute( null );
    }

    private async void OnGroupOrderCreated()
    {
        await Shell.Current.GoToAsync( "orders" );
    }
}
