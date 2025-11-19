using BatchPay.Models;
using BatchPay.ViewModels;
using FrontendServices;

namespace BatchPay.Pages
{
    public partial class VennerPage : ContentPage
    {
        private readonly VennerPageViewModel _vm;
        private DateTime _lastQueryAt = DateTime.MinValue;
        private bool _allUsersLoaded = false;

        public VennerPage()
        {
            InitializeComponent();

            _vm = new VennerPageViewModel( new BatchPayService() );
            BindingContext = _vm;

            // Default: Venner-fanen
            SetTab( true );

            // Hent venneliste ved opstart
            _ = _vm.LoadFriendsAsync();
        }

        // ---------- Faner ----------
        private void SetTab( bool isFriends )
        {
            if (isFriends)
            {
                TabFriendsBtn.BackgroundColor = (Color)Application.Current.Resources[ "ColorRow" ];
                TabFriendsBtn.TextColor = (Color)Application.Current.Resources[ "ColorText" ];
                TabAllBtn.BackgroundColor = Colors.Transparent;
                TabAllBtn.TextColor = (Color)Application.Current.Resources[ "ColorMuted" ];
            }
            else
            {
                TabAllBtn.BackgroundColor = (Color)Application.Current.Resources[ "ColorRow" ];
                TabAllBtn.TextColor = (Color)Application.Current.Resources[ "ColorText" ];
                TabFriendsBtn.BackgroundColor = Colors.Transparent;
                TabFriendsBtn.TextColor = (Color)Application.Current.Resources[ "ColorMuted" ];
            }

            FriendsView.IsVisible = isFriends;
            AllUsersView.IsVisible = !isFriends;
            GlobalSearchBarHost.IsVisible = !isFriends;
        }

        private async void OnTabFriendsClicked( object sender, EventArgs e )
        {
            SetTab( true );
            await _vm.RefreshFriendsIfEmptyAsync();
        }

        private async void OnTabAllClicked( object sender, EventArgs e )
        {
            SetTab( false );

                await _vm.LoadAllUsersAsync();    // hent global liste
                _vm.FilterGlobal( "" );             // vis alle
            
        }

        // ---------- Global søg (på "Alle") ----------
        private async void OnGlobalSearchTextChanged( object sender, TextChangedEventArgs e )
        {
            var text = (e.NewTextValue ?? string.Empty).Trim();

            // Debounce ~200ms
            var now = DateTime.UtcNow;
            if ((now - _lastQueryAt).TotalMilliseconds < 200) return;
            _lastQueryAt = now;

            _vm.FilterGlobal( text );
            await System.Threading.Tasks.Task.CompletedTask;
        }

        // ---------- Tilføj / Fjern ven ----------
        private async void OnAddFriendClicked( object sender, EventArgs e )
        {
            var btn = sender as Button;
            var user = btn == null ? null : btn.CommandParameter as UserLite;
            if (user == null) return;

            var ok = await _vm.AddFriendAsync( user );

            // Ingen popup – lille visuel bekræftelse
            if (ok && btn != null)
            {
                btn.Text = "✓";
                await System.Threading.Tasks.Task.Delay( 600 );
                btn.Text = "+";
            }
        }

        private async void OnRemoveFriendClicked( object sender, EventArgs e )
        {
            var btn = sender as Button;
            var user = btn == null ? null : btn.CommandParameter as UserLite;
            if (user == null) return;

            var ok = await _vm.RemoveFriendAsync( user );
            if (ok && btn != null)
            {
                btn.Text = "✓";
                await System.Threading.Tasks.Task.Delay( 500 );
                btn.Text = "−";
            }
        }
    }
}
