using System.Collections.ObjectModel;
using System.Windows.Input;
using BatchPay.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontendServices;

namespace BatchPay.ViewModels;

public partial class GroupPaymentViewModel : ObservableObject
{
    [ObservableProperty] private MerchantModel? selectedMerchant;
    public ObservableCollection<MerchantModel> Merchants { get; } = new();

    private readonly IBatchPayService _batchPayService;

    [ObservableProperty]
    private string _title = "Ny gruppebetaling";

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private bool _isBusy;

    public ObservableCollection<FriendSelectionItem> Friends { get; } = new();

    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand CreateGroupOrderCommand { get; }

    private bool _hasLoaded;


    public GroupPaymentViewModel( IBatchPayService batchPayService )
    {
        _batchPayService = batchPayService;

        LoadCommand = new AsyncRelayCommand( LoadAsync );
        CreateGroupOrderCommand = new AsyncRelayCommand( CreateGroupOrderAsync );

    }


    private async Task LoadAsync()
    {
        if (_hasLoaded) return; // kun første gang siden vises
        _hasLoaded = true;

        try
        {
            IsBusy = true;
            StatusMessage = "Henter dine venner...";

            var friends = await _batchPayService.GetFriendsAsync();

            Friends.Clear();
            foreach (var f in friends)
            {
                Friends.Add( new FriendSelectionItem
                {
                    UserId = f.UserId,
                    DisplayName = f.FullName // tilpas til dit UserLite-felt
                } );
            }

            StatusMessage = "Vælg de venner der skal med i gruppebetalingen.";
        }
        catch (Exception ex)
        {
            StatusMessage = "Kunne ikke hente venner: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CreateGroupOrderAsync()
    {
        if (IsBusy) return;

        var selected = Friends.Where( f => f.IsSelected ).Select( f => f.UserId ).ToList();
        if (selected.Count == 0)
        {
            StatusMessage = "Vælg mindst én ven.";
            return;
        }

        try
        {
            IsBusy = true;
            StatusMessage = "Opretter gruppebetaling...";

            var request = new CreateGroupOrderRequest
            {
                OwnerUserId = 13, // samme som X-UserId header lige nu
                ParticipantUserIds = selected,
                Title = Title,
                MerchantId = null // kommer senere
            };

            var response = await _batchPayService.CreateGroupOrderAsync( request );

            if (response == null)
            {
                StatusMessage = "API svarede ikke som forventet.";
                return;
            }

            StatusMessage = $"Gruppebetaling oprettet. Id: {response.GroupOrderId}, kode: {response.GroupOrderCode}";
        }
        catch (Exception ex)
        {
            StatusMessage = "Fejl under oprettelse: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
