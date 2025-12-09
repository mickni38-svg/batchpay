using BatchPay;
using BatchPay.Pages;
using BatchPay.ViewModels;
using CommunityToolkit.Maui;
using FrontendServices;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()   // ← DEN HER MANGLEDE
            .UseMauiCommunityToolkit()
            .ConfigureFonts( fonts =>
            {
                fonts.AddFont( "OpenSans-Regular.ttf", "OpenSansRegular" );
                fonts.AddFont( "OpenSans-Semibold.ttf", "OpenSansSemibold" );
            } );

        // Views
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<GroupPaymentPage>();
        builder.Services.AddTransient<NotificationsPage>();
        builder.Services.AddTransient<VennerPage>();

        // ViewModels
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<GroupPaymentViewModel>();
        builder.Services.AddTransient<NotificationsViewModel>();
        builder.Services.AddTransient<VennerPageViewModel>();

        // (valgfrit) Brug DI til Shell
        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddSingleton<IBatchPayService, BatchPayService>();

        builder.Services.AddSingleton<IBatchPayService, BatchPayService>();

        return builder.Build();
    }
}
