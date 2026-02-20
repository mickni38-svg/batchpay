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

public partial class CreateGroupViewModel : ObservableObject
{
    private readonly BatchPayApiClient _api;

    // TODO: senere skal dette komme fra login/session
    public int CurrentUserId { get; } = 1;

    public ObservableCollection<SelectableDirectoryEntry> PotentialMembers { get; } = new();

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string message = string.Empty;

    [ObservableProperty]
    private string iconKey = "users"; // Default icon

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? statusMessage;

    public bool HasSelection => PotentialMembers.Any( x => x.IsSelected );

    public CreateGroupViewModel( BatchPayApiClient api )
    {
        _api = api;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        StatusMessage = null;
        PotentialMembers.Clear();

        try
        {
            var friends = await _api.GetFriendsAsync( CurrentUserId, CancellationToken.None );

            foreach (var friend in friends)
            {
                if (friend.Id == CurrentUserId)
                    continue;

                var dto = new DirectoryEntryDto(
                    DirectoryEntryType.User,
                    friend.Id,
                    friend.DisplayName,
                    friend.Handle );

                var entry = new SelectableDirectoryEntry( dto )
                {
                    IsSelected = false
                };

                PotentialMembers.Add( entry );
            }

            OnPropertyChanged( nameof( HasSelection ) );
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading members: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void ToggleSelect( SelectableDirectoryEntry? entry )
    {
        if (entry is null || !entry.IsSelectable) return;

        entry.IsSelected = !entry.IsSelected;
        OnPropertyChanged( nameof( HasSelection ) );
    }

    [RelayCommand]
    public async Task CreateGroupAsync()
    {
        if (IsBusy || !HasSelection || string.IsNullOrWhiteSpace( Title ))
        {
            StatusMessage = "Title and at least one member are required.";
            return;
        }

        IsBusy = true;
        StatusMessage = null;

        try
        {
            var selectedMemberIds = PotentialMembers
                .Where( m => m.IsSelected )
                .Select( m => m.Id )
                .ToList();

            var request = new CreateGroupPaymentRequestDto(
                Title,
                Message,
                CurrentUserId,
                selectedMemberIds,
                IconKey,
                IsActive: true
            );

            await _api.CreateGroupPaymentAsync( request, CancellationToken.None );

            StatusMessage = "Group created successfully!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error creating group: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
