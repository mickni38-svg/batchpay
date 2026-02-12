using System;
using Microsoft.Maui.Controls;

namespace BatchPay.Frontend.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnOverblikTapped( object sender, EventArgs e )
        => await Shell.Current.GoToAsync( "//overview" );

    private async void OnOpretTapped( object sender, EventArgs e )
        => await Shell.Current.GoToAsync( "//create" );

    private async void OnBrugereTapped( object sender, EventArgs e )
        => await Shell.Current.GoToAsync( "//find" );

    private async void OnBeskederTapped( object sender, EventArgs e )
        => await Shell.Current.GoToAsync( "//messages" );

    private async void OnLoginClicked( object sender, EventArgs e )
        => await DisplayAlert( "Log ind", "Login kommer senere 🙂", "OK" );

    private async void OnSignupTapped( object sender, EventArgs e )
        => await DisplayAlert( "Opret dig", "Opret dig som bruger kommer senere 🙂", "OK" );
}
