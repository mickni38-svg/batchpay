using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using BatchPay.Contracts.Dto;
using BatchPay.Frontend.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatchPay.Frontend.ViewModels;

public partial class GroupPaymentsViewModel : ObservableObject
{
    private readonly BatchPayApiClient _api;

    // TODO: senere skal dette komme fra login/session
    public int CurrentUserId { get; } = 1;

    public ObservableCollection<GroupPaymentDto> GroupPayments { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? statusMessage;

    public GroupPaymentsViewModel(BatchPayApiClient api)
    {
        _api = api;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        StatusMessage = null;
        GroupPayments.Clear();

        try
        {
            // The API call itself doesn't need to change, but the DTO it returns is different.
            var groups = await _api.GetGroupPaymentsForUserAsync(CurrentUserId, CancellationToken.None);
            foreach (var group in groups)
            {
                // The GroupPaymentDto now contains a list of members that can be
                // users or merchants. The UI can bind to this list directly.
                GroupPayments.Add(group);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading group payments: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeactivateGroupAsync(int groupPaymentId)
    {
        if (IsBusy) return;

        IsBusy = true;
        StatusMessage = null;

        try
        {
            await _api.DeactivateGroupPaymentAsync(groupPaymentId, CancellationToken.None);

            // Reload the list to reflect the change.
            await LoadAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deactivating group: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}