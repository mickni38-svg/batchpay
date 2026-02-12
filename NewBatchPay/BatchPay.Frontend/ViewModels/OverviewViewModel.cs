using BatchPay.Contracts.Dto;
using BatchPay.Frontend.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;

namespace BatchPay.Frontend.ViewModels;

public partial class OverviewViewModel : ObservableObject
{
    private readonly BatchPayApiClient _api;

    public int CurrentUserId { get; } = 1;

    public ObservableCollection<GroupPaymentTabVm> Tabs { get; } = new();
    public ObservableCollection<GroupPaymentMemberVm> MemberAccordions { get; } = new();

    private GroupPaymentDto? _selectedGroupPayment;

    public GroupPaymentDto? SelectedGroupPayment
    {
        get => _selectedGroupPayment;
        private set
        {
            if (ReferenceEquals( _selectedGroupPayment, value ))
                return;

            _selectedGroupPayment = value;
            OnPropertyChanged();

            RebuildMembers();

            // Notify "message" bindings
            OnPropertyChanged( nameof( SelectedMessage ) );
            OnPropertyChanged( nameof( HasSelectedMessage ) );
            OnPropertyChanged( nameof( HasNoSelectedMessage ) );
        }
    }

    public string SelectedMessage => SelectedGroupPayment?.Message?.Trim() ?? "";
    public bool HasSelectedMessage => !string.IsNullOrWhiteSpace( SelectedMessage );
    public bool HasNoSelectedMessage => !HasSelectedMessage;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? statusMessage;

    public OverviewViewModel( BatchPayApiClient api )
    {
        _api = api;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        StatusMessage = null;

        try
        {
            var gps = await _api.GetGroupPaymentsForUserAsync( CurrentUserId, CancellationToken.None );

            MainThread.BeginInvokeOnMainThread( () =>
            {
                Tabs.Clear();

                foreach (var gp in gps)
                    Tabs.Add( new GroupPaymentTabVm( gp ) );

                for (int i = 0; i < Tabs.Count; i++)
                    Tabs[ i ].ShowDivider = i != Tabs.Count - 1;

                var first = Tabs.FirstOrDefault();
                if (first is not null)
                    SelectGroupPayment( first );
                else
                    ClearMembers();
            } );
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

    [RelayCommand]
    private void SelectGroupPayment( GroupPaymentTabVm tab )
    {
        foreach (var t in Tabs)
            t.IsSelected = false;

        tab.IsSelected = true;
        SelectedGroupPayment = tab.Model;
    }

    [RelayCommand]
    private async Task DeleteGroupPaymentAsync( GroupPaymentTabVm tab )
    {
        if (tab?.Model is null)
            return;

        var ok = await Shell.Current.DisplayAlert(
            "Slet",
            $"Vil du slette \"{tab.Model.Title}\"?\n(Bliver kun deaktiveret)",
            "Slet",
            "Annuller" );

        if (!ok)
            return;

        try
        {
            IsBusy = true;
            StatusMessage = null;

            await _api.DeactivateGroupPaymentAsync( tab.Model.Id, CancellationToken.None );

            // Reload så chip forsvinder
            await LoadAsync();
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

    [RelayCommand]
    private async Task PayOrderAsync()
    {
        if (SelectedGroupPayment is null)
            return;

        await Shell.Current.DisplayAlert( "Betal ordre", $"Valgt: {SelectedGroupPayment.Title}", "OK" );
    }

    private void RebuildMembers()
    {
        MemberAccordions.Clear();

        var members = SelectedGroupPayment?.Members;
        if (members is null || members.Count == 0)
            return;

        foreach (var u in members)
            MemberAccordions.Add( new GroupPaymentMemberVm( u ) );
    }

    private void ClearMembers()
    {
        SelectedGroupPayment = null;
        MemberAccordions.Clear();
    }
}

public partial class GroupPaymentTabVm : ObservableObject
{
    public GroupPaymentDto Model { get; }

    public string Title => Model.Title;

    // Mapper IconKey -> MAUI asset filename (uden .svg er mest stabilt)
    public string IconSource => $"icon_{Model.IconKey}";

    [ObservableProperty] private bool isSelected;
    [ObservableProperty] private bool showDivider = true;

    public GroupPaymentTabVm( GroupPaymentDto model )
    {
        Model = model;
    }
}

public partial class GroupPaymentMemberVm : ObservableObject
{
    public UserDto User { get; }
    public string Initials { get; }
    public Color AvatarColor { get; }

    [ObservableProperty] private bool isExpanded;

    public GroupPaymentMemberVm( UserDto user )
    {
        User = user;

        Initials = CreateInitials( user.DisplayName );
        AvatarColor = CreateAvatarColor( user.Id );
    }

    [RelayCommand]
    private void Toggle()
    {
        IsExpanded = !IsExpanded;
    }

    private static string CreateInitials( string displayName )
    {
        var parts = (displayName ?? "").Trim().Split( ' ', StringSplitOptions.RemoveEmptyEntries );

        if (parts.Length == 0) return "?";
        if (parts.Length == 1)
            return parts[ 0 ].Substring( 0, Math.Min( 2, parts[ 0 ].Length ) ).ToUpperInvariant();

        var first = parts[ 0 ].Substring( 0, 1 );
        var last = parts[ ^1 ].Substring( 0, 1 );
        return (first + last).ToUpperInvariant();
    }

    private static Color CreateAvatarColor( int userId )
    {
        int r = (userId * 53) % 120 + 40;
        int g = (userId * 97) % 120 + 40;
        int b = (userId * 193) % 120 + 40;
        return Color.FromRgb( r, g, b );
    }
}
