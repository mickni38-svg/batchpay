using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BatchPay.Models;
using BatchPay.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontendServices;

namespace BatchPay.ViewModels;

public partial class GroupPaymentCreateViewModel : ObservableObject
{
    public event Action? GroupOrderCreated;

    private readonly IBatchPayService _service;
    private readonly GroupOrderStore _orderStore;

    public event Action? CreatedSuccessfully;
    public ObservableCollection<MerchantModel> Merchants { get; } = new();
    public ObservableCollection<UserLite> AllFriends { get; } = new();
    public ObservableCollection<UserLite> FilteredFriends { get; } = new();
    public ObservableCollection<UserLite> SelectedFriends { get; } = new();

    [ObservableProperty]
    private string title = "";

    [ObservableProperty]
    private MerchantModel? selectedMerchant;

    [ObservableProperty]
    private string friendSearchText = "";

    [ObservableProperty]
    private string? statusMessage;

    [ObservableProperty]
    private bool isBusy;

    private bool _loaded;

    public GroupPaymentCreateViewModel( IBatchPayService service, GroupOrderStore orderStore )
    {
        _service = service;
        _orderStore = orderStore;

        LoadMerchantsDummy();
        SelectedMerchant = Merchants.FirstOrDefault();
    }

    /// <summary>
    /// Kald denne når siden vises første gang.
    /// </summary>
    [RelayCommand]
    public async Task LoadAsync()
    {
        if (_loaded) return;
        _loaded = true;

        try
        {
            IsBusy = true;
            StatusMessage = "Henter dine venner...";

            var friends = await _service.GetFriendsAsync();

            AllFriends.Clear();
            FilteredFriends.Clear();

            foreach (var f in friends)
            {
                AllFriends.Add( f );
                FilteredFriends.Add( f );
            }

            StatusMessage = null;
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

    [RelayCommand]
    private void SearchFriends()
    {
        FilteredFriends.Clear();

        if (string.IsNullOrWhiteSpace( FriendSearchText ))
        {
            foreach (var f in AllFriends)
                FilteredFriends.Add( f );
            return;
        }

        var q = FriendSearchText.Trim();

        // ⚠️ Ret DisplayName hvis dit UserLite bruger et andet felt, fx Name/Username
        var matches = AllFriends.Where( f =>
            (f.FullName ?? "").Contains( q, StringComparison.OrdinalIgnoreCase ) );

        foreach (var f in matches)
            FilteredFriends.Add( f );
    }

    [RelayCommand]
    private void AddFriend( UserLite friend )
    {
        if (friend == null) return;
        if (SelectedFriends.Any( x => x.UserId == friend.UserId )) return; // ⚠️ Ret UserId hvis andet felt

        SelectedFriends.Add( friend );
        StatusMessage = null;
    }

    [RelayCommand]
    private void RemoveFriend( UserLite friend )
    {
        if (friend == null) return;

        var existing = SelectedFriends.FirstOrDefault( x => x.UserId == friend.UserId ); // ⚠️ Ret UserId hvis andet felt
        if (existing != null)
            SelectedFriends.Remove( existing );
    }

    [RelayCommand]
    private async Task CreateGroup()
    {
        if (IsBusy) return;

        // Validering (frontend)
        if (string.IsNullOrWhiteSpace( Title ))
        {
            StatusMessage = "Indtast et navn på gruppebetalingen.";
            return;
        }

        if (SelectedMerchant == null)
        {
            StatusMessage = "Vælg en virksomhed.";
            return;
        }

        if (SelectedFriends.Count == 0)
        {
            StatusMessage = "Tilføj mindst én deltager.";
            return;
        }

        try
        {
            IsBusy = true;
            StatusMessage = "Opretter gruppebetaling...";

            // OwnerUserId – lige nu bruger du X-UserId=13 i din BatchPayService header.
            // Så vi sætter 13 her for at holde det konsistent.
            var request = new CreateGroupOrderRequest
            {
                OwnerUserId = 13,
                Title = Title.Trim(),
                MerchantId = SelectedMerchant.Id,
                ParticipantUserIds = SelectedFriends.Select( f => f.UserId ).ToList() // ⚠️ Ret UserId hvis andet felt
            };

            var result = await _service.CreateGroupOrderAsync( request );

            if (result == null)
            {
                StatusMessage = "Noget gik galt. Kunne ikke oprette gruppebetalingen.";
                return;
            }

            // ✅ SUCCESS: vi har et resultat, nu gemmer vi den i frontend-listen
            _orderStore.Add( new GroupOrderSummary
            {
                GroupOrderId = result.GroupOrderId,
                GroupOrderCode = result.GroupOrderCode,
                Title = Title.Trim(),
                MerchantName = SelectedMerchant!.Name,
                ParticipantsCount = SelectedFriends.Count,
                Status = "Afventer",
                CreatedAt = DateTime.Now
            } );

            System.Diagnostics.Debug.WriteLine( $"[STORE] Orders count = {_orderStore.Orders.Count}" );
            StatusMessage = $"Gruppebetaling oprettet! Kode: {result.GroupOrderCode} (Id: {result.GroupOrderId})";

            // (Valgfrit) trigger navigation
            //CreatedSuccessfully?.Invoke();
            GroupOrderCreated?.Invoke();


            // (Valgfrit) Ryd form efter oprettelse:
            // Title = "";
            // SelectedFriends.Clear();
        }
        catch (Exception ex)
        {
            StatusMessage = "Fejl: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LoadMerchantsDummy()
    {
        Merchants.Clear();

        // Kun dem med abonnement skal vises – her er de allerede filtreret.
        Merchants.Add( new MerchantModel { Id = "TONYS", Name = "Tony's Pizza", Category = "Pizza", City = "Aarhus", HasSubscription = true } );
        Merchants.Add( new MerchantModel { Id = "BURGERHUT", Name = "BurgerHut", Category = "Burger", City = "Aarhus", HasSubscription = true } );
        Merchants.Add( new MerchantModel { Id = "SUSHIHOUSE", Name = "SushiHouse", Category = "Sushi", City = "Aarhus", HasSubscription = true } );
    }
}
