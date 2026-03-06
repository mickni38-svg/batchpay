using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatchPay.Contracts.Dto;
using BatchPay.Frontend.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatchPay.Frontend.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly BatchPayApiClient _api;
    private readonly IUserContext _userContext;

    public ObservableCollection<UserDto> Users { get; } = new();

    [ObservableProperty]
    private UserDto? selectedUser;

    // (valgfrit) viser at “login virker”
    public ObservableCollection<UserDto> Friends { get; } = new();

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? statusMessage;

    public bool IsLoggedIn => _userContext.CurrentUserId is not null;

    public string LoggedInAsText
    => SelectedUser is null ? "Ikke logget ind" : $"Logget ind som: {SelectedUser.DisplayName}";

    public HomeViewModel( BatchPayApiClient api, IUserContext userContext )
    {
        _api = api;
        _userContext = userContext;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        StatusMessage = null;

        try
        {
            Users.Clear();

            var users = await _api.GetAllUsersAsync( CancellationToken.None );
            foreach (var u in users.OrderBy( x => x.DisplayName ))
                Users.Add( u );

            // Hvis der allerede er valgt en bruger (Preferences), så vælg den i dropdown
            var currentId = _userContext.CurrentUserId;
            if (currentId is not null)
            {
                SelectedUser = Users.FirstOrDefault( x => x.Id == currentId.Value );
                OnPropertyChanged( nameof( IsLoggedIn ) );
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading users: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task LoginAsync()
    {
        if (IsBusy) return;

        if (SelectedUser is null)
        {
            StatusMessage = "Vælg en bruger først.";
            return;
        }

        IsBusy = true;
        StatusMessage = null;

        try
        {
            _userContext.CurrentUserId = SelectedUser.Id;
            OnPropertyChanged( nameof( IsLoggedIn ) );

            // (valgfrit) hent venner for at verificere at alt skifter pr. bruger
            Friends.Clear();
            var friends = await _api.GetFriendsAsync( SelectedUser.Id, CancellationToken.None );
            foreach (var f in friends)
                Friends.Add( f );

            StatusMessage = $"Logget ind som: {SelectedUser.DisplayName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Login failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedUserChanged( UserDto? value )
    {
        OnPropertyChanged( nameof( LoggedInAsText ) );
    }

    [RelayCommand]
    public void Logout()
    {
        _userContext.CurrentUserId = null;
        SelectedUser = null;
        Friends.Clear();
        StatusMessage = "Logget ud.";
        OnPropertyChanged( nameof( IsLoggedIn ) );
    }
}