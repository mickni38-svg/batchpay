using BatchPay.Frontend.ViewModels;

namespace BatchPay.Frontend.Views;

public partial class CreateGroupPaymentPage : ContentPage
{
    public CreateGroupPaymentPage( CreateGroupPaymentViewModel vm )
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CreateGroupPaymentViewModel vm)
        {
            // CommunityToolkit: [RelayCommand] private Task LoadAsync()
            // => genererer public IAsyncRelayCommand LoadCommand
            if (vm.LoadCommand?.CanExecute( null ) == true)
                vm.LoadCommand.Execute( null );
        }
    }
}
