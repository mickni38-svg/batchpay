using System;
using System.Collections.ObjectModel;
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

    // TODO: senere skal dette komme fra login/session
    public int CurrentUserId { get; } = 1;

    // This list will now contain both Users and Merchants who are friends.
    public ObservableCollection<UserDto> Friends { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? statusMessage;

    public HomeViewModel(BatchPayApiClient api)
    {
        _api = api;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        StatusMessage = null;
        Friends.Clear();

        try
        {
            // MODIFIED: The API call is simpler and the parameter name is updated.
            // The returned UserDto list can now contain both users and merchants.
            var friends = await _api.GetFriendsAsync(CurrentUserId, CancellationToken.None);
            foreach (var friend in friends)
            {
                Friends.Add(friend);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading friends: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}