using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatchPay.Contracts.Dto;
using BatchPay.Frontend.Models;
using BatchPay.Frontend.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatchPay.Frontend.ViewModels;

public partial class CreateGroupPaymentViewModel : ObservableObject
{
    private readonly BatchPayApiClient _api;

    // TODO: senere skal dette komme fra login/session
    public int CurrentUserId { get; } = 1;

    // IMPORTANT: ikon-grid binder til denne
    public ObservableCollection<IconOptionVm> Icons { get; } = new();

    // Genbrug samme "SelectableUser" som FindPersonsPage,
    // så layout + selection-ring kan være identisk.
    public ObservableCollection<SelectableUser> Friends { get; } = new();
    public ObservableCollection<SelectableUser> FilteredFriends { get; } = new();

    [ObservableProperty] private string friendSearchText = "";
    [ObservableProperty] private string title = "";
    [ObservableProperty] private string message = "";
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? statusMessage;

    public bool HasSelection => Friends.Any( x => x.IsSelected );

    public string SelectedIconKey => Icons.FirstOrDefault( x => x.IsSelected )?.Key ?? "other";

    public IEnumerable<int> SelectedMemberUserIds =>
        Friends.Where( x => x.IsSelected ).Select( x => x.Id );

    public CreateGroupPaymentViewModel( BatchPayApiClient api )
    {
        _api = api;

        // IKONER: Sørg for at dine filer hedder icon_{key}.svg og ligger i Resources/Images
        var keys = new[]
        {
            "pizza","beer","trip","party","work","coffee","burger","sushi","taco","music","movie",
            "food","cafe","education","fun","gift","health","housing","other","shopping","transport","travel","utilities"
        };

        foreach (var key in keys)
            Icons.Add( new IconOptionVm( key, $"icon_{key}.svg" ) );

        // Default valgt ikon
        var first = Icons.FirstOrDefault();
        if (first != null) first.IsSelected = true;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            StatusMessage = null;

            Friends.Clear();
            FilteredFriends.Clear();

            var friends = await _api.GetFriendsAsync( CurrentUserId, CancellationToken.None );

            foreach (var f in friends)
                Friends.Add( new SelectableUser( f ) );

            ApplyFriendFilter();
            OnPropertyChanged( nameof( HasSelection ) );
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnFriendSearchTextChanged( string value )
    {
        ApplyFriendFilter();
    }

    private void ApplyFriendFilter()
    {
        FilteredFriends.Clear();

        var q = (FriendSearchText ?? "").Trim();

        var list = string.IsNullOrWhiteSpace( q )
            ? Friends
            : Friends.Where( x =>
                x.DisplayName.Contains( q, StringComparison.OrdinalIgnoreCase ) ||
                x.Handle.Contains( q, StringComparison.OrdinalIgnoreCase ) );

        foreach (var u in list)
            FilteredFriends.Add( u );
    }

    [RelayCommand]
    public void OpenFriendSearch()
    {
        ApplyFriendFilter();
    }

    [RelayCommand]
    private void ToggleFriend( SelectableUser item )
    {
        if (item is null) return;

        item.IsSelected = !item.IsSelected;
        OnPropertyChanged( nameof( HasSelection ) );
    }

    [RelayCommand]
    private void SelectIcon( IconOptionVm icon )
    {
        if (icon is null) return;

        foreach (var i in Icons)
            i.IsSelected = false;

        icon.IsSelected = true;
    }

    [RelayCommand]
    private async Task CreateAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            StatusMessage = null;

            if (string.IsNullOrWhiteSpace( Title ))
            {
                StatusMessage = "Titel mangler.";
                return;
            }

            var request = new CreateGroupPaymentRequestDto(
                CreatedByUserId: CurrentUserId,
                Title: Title.Trim(),
                Message: Message?.Trim(),
                IconKey: SelectedIconKey,
                IsActive: true,
                CreatedAtUtc: DateTime.UtcNow,
                MemberUserIds: SelectedMemberUserIds.ToList()
            );

            await _api.CreateGroupPaymentAsync( request, CancellationToken.None );

            await Shell.Current.DisplayAlert( "Oprettet", "Bestilling oprettet", "OK" );

            // Efter OK -> hop automatisk til Oversigt-tab
            await Shell.Current.GoToAsync( "//overview" );
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}

/// <summary>
/// IMPORTANT: En ikon-option i grid.
/// </summary>
public partial class IconOptionVm : ObservableObject
{
    public string Key { get; }
    public string ImageSource { get; }

    [ObservableProperty] private bool isSelected;

    public IconOptionVm( string key, string imageSource )
    {
        Key = key;
        ImageSource = imageSource;
    }
}
