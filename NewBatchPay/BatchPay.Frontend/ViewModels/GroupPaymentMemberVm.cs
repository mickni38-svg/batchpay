using BatchPay.Contracts.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BatchPay.Frontend.ViewModels;

public partial class GroupPaymentMemberVm : ObservableObject
{
    public bool IsMerchant { get; }
    public int UserId { get; }
    public string Initials { get; }
    public Color AvatarColor { get; }
    public string DisplayName { get; set; }
    public string MerchantDisplayName { get; }

    [ObservableProperty] private bool isExpanded;

    // Accordion data
    public ObservableCollection<OrderLineVm> OrderLines { get; } = new();

    [ObservableProperty] private string? orderTitle;

    public bool HasOrder { get;  set; }

    public string OrderTotalText
    {
        get
        {
            var total = OrderLines.Sum( x => x.Quantity * x.UnitPrice );
            return $"{total:0.00} kr";
        }
    }

    public string PaymentStatusText => HasOrder ? "Betalt" : "Afventer";

    public GroupPaymentMemberVm( DirectoryEntryDto user )
    {
        IsMerchant = user.Type == DirectoryEntryType.Merchant;
        UserId = user.Id;
         Initials = CreateInitials( user.DisplayName );
        AvatarColor = CreateAvatarColor( user.Id );
        DisplayName = user.DisplayName;
               
        OrderLines.CollectionChanged += OnOrderLinesChanged;

        // ✅ Start empty – bliver fyldt af OverviewViewModel via DB-kald
        OrderTitle = null;
    }

    private void OnOrderLinesChanged( object? sender, NotifyCollectionChangedEventArgs e )
    {
        OnPropertyChanged( nameof( HasOrder ) );
        OnPropertyChanged( nameof( OrderTotalText ) );
        OnPropertyChanged( nameof( PaymentStatusText ) );
    }

    public void ApplyLatestOrder( OrderDto? order )
    {
        if (order is null)
        {
            //OrderTitle = order?.MerchantName;
            OrderLines.Clear();
            OnPropertyChanged( nameof( HasOrder ) );
            OnPropertyChanged( nameof( OrderTotalText ) );
            OnPropertyChanged( nameof( PaymentStatusText ) );
            return;
        }

       // OrderTitle = $"Ordre #{order.Id}";
        OrderLines.Clear();

        foreach (var l in order.Lines)
            OrderLines.Add( new OrderLineVm( l.ItemName, l.Quantity, l.UnitPrice ) );

        HasOrder = OrderLines.Count > 0;

        OnPropertyChanged( nameof( HasOrder ) );
        OnPropertyChanged( nameof( OrderTotalText ) );
        OnPropertyChanged( nameof( PaymentStatusText ) );
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

    public string TabText => $"{OrderTitle} · {DisplayName}";
}

public sealed partial class OrderLineVm : ObservableObject
{
    public OrderLineVm( string itemName, int quantity, decimal unitPrice )
    {
        ItemName = itemName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public string ItemName { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }

    public string UnitPriceText => $"{UnitPrice:0.00} kr";
}