using BatchPay.ViewModels;

namespace BatchPay.Pages;

public partial class GroupPaymentPage : ContentPage
{
    private readonly GroupPaymentViewModel _vm;

    public GroupPaymentPage( GroupPaymentViewModel vm )
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadCommand.Execute( null );   // henter venner når siden vises
    }
}
