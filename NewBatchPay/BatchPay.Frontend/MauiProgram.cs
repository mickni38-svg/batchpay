using BatchPay.Frontend.DevTools;
using BatchPay.Frontend.Services;
using BatchPay.Frontend.ViewModels;
using BatchPay.Frontend.Views;

namespace BatchPay.Frontend;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.UseMauiApp<App>();
        // inside CreateMauiApp() before builder.Build()
#if ANDROID
        var apiBase = "http://10.0.2.2:5000/"; // emulator -> host machine
#else
var apiBase = "http://localhost:5000/";
#endif

        //opret ikoner
#if DEBUG
        //DevIconSeeder.EnsureIconsExist();
#endif


        // Android emulator -> host localhost = 10.0.2.2
        builder.Services.AddSingleton( _ => new HttpClient
        {
            BaseAddress = new Uri( "http://10.0.2.2:5000/" )
        } );

        builder.Services.AddSingleton<BatchPayApiClient>();

        //builder.Services.AddTransient<CreateGroupPaymentViewModel>();
        builder.Services.AddTransient<FindPersonsViewModel>();
        builder.Services.AddTransient<CreateGroupViewModel>();
        builder.Services.AddTransient<OverviewViewModel>();
        builder.Services.AddTransient<GroupPaymentsViewModel>();
        //builder.Services.AddTransient<MessagesViewModel>();

        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<CreateGroupPaymentPage>();
        builder.Services.AddTransient<FindPersonsPage>();
        builder.Services.AddTransient<OverviewPage>();


        //builder.Services.AddTransient<MessagesPage>();
        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }
}
