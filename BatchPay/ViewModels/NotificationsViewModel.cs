using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BatchPay.ViewModels;

public enum RequestKind
{
    FriendIncoming,   // Modtaget venneanmodning
    FriendSent,       // Sendt venneanmodning (afventer) — kan annulleres
    GroupInvite       // Invitation til gruppebetaling — tilmeld/annullér
}

public class RequestItem
{
    public int Id { get; set; }
    public RequestKind Kind { get; set; }

    /// <summary>
    /// Primær overskrift i UI, fx brugernavn eller gruppetitel.
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// Sekundær tekst (valgfri), fx "Inviteret af Laura" eller "afventer godkendelse".
    /// </summary>
    public string Subtitle { get; set; } = "";

    public DateTime SentAtUtc { get; set; }

    /// <summary>
    /// Viser "Venneanmodning" eller "Gruppebetalingsanmodning"
    /// </summary>
    public string TypeLabel =>
        Kind switch
        {
            RequestKind.FriendIncoming => "Venneanmodning",
            RequestKind.FriendSent => "Venneanmodning",
            RequestKind.GroupInvite => "Gruppebetalingsanmodning",
            _ => "Anmodning"
        };
}

public partial class NotificationsViewModel : ObservableObject
{
    public string Title => "Notifikationer";
    public ObservableCollection<RequestItem> Requests { get; } = new();

    public NotificationsViewModel()
    {
        // 🔹 Dummydata
        Requests.Add( new RequestItem
        {
            Id = 11,
            Kind = RequestKind.FriendIncoming,
            Title = "Jonas K.",
            Subtitle = "har sendt dig en venneanmodning",
            SentAtUtc = DateTime.UtcNow.AddMinutes( -30 )
        } );
        Requests.Add( new RequestItem
        {
            Id = 12,
            Kind = RequestKind.FriendIncoming,
            Title = "Sofie D.",
            Subtitle = "har sendt dig en venneanmodning",
            SentAtUtc = DateTime.UtcNow.AddHours( -5 )
        } );
        Requests.Add( new RequestItem
        {
            Id = 1,
            Kind = RequestKind.FriendSent,
            Title = "Maja H.",
            Subtitle = "afventer godkendelse",
            SentAtUtc = DateTime.UtcNow.AddHours( -3 )
        } );
        Requests.Add( new RequestItem
        {
            Id = 21,
            Kind = RequestKind.GroupInvite,
            Title = "Tonys Pizza",
            Subtitle = "Inviteret af Laura",
            SentAtUtc = DateTime.UtcNow.AddMinutes( -25 )
        } );
        Requests.Add( new RequestItem
        {
            Id = 22,
            Kind = RequestKind.GroupInvite,
            Title = "Fredagsbar",
            Subtitle = "Inviteret af Ali",
            SentAtUtc = DateTime.UtcNow.AddHours( -2 )
        } );
    }

    [RelayCommand] private Task RefreshAsync() => Task.Delay( 300 );

    // ✅ Godkend/Tilmeld
    [RelayCommand]
    private Task ApproveRequestAsync( RequestItem item )
    {
        // POC: fjern elementet — i “rigtig” app ville du kalde API først.
        Requests.Remove( item );
        return Task.CompletedTask;
    }

    // ❌ Afvis/Annullér
    [RelayCommand]
    private Task DeclineRequestAsync( RequestItem item )
    {
        Requests.Remove( item );
        return Task.CompletedTask;
    }
}
