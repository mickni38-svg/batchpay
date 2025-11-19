using BatchPay.Models;   // nu her
using FrontendServices;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace BatchPay.ViewModels
{
    public class VennerPageViewModel( IBatchPayService api ) : INotifyPropertyChanged
    {
        private readonly IBatchPayService _api = api;

        // Personlig venneliste (default fane)
        public ObservableCollection<UserLite> Friends { get; private set; } = new ObservableCollection<UserLite>();

        // Globalt brugerindeks (fra /api/users)
        public ObservableCollection<UserLite> GlobalUsers { get; private set; } = new ObservableCollection<UserLite>();

        // Filtreret udsnit af global liste (vises på "Alle"-fanen)
        public ObservableCollection<UserLite> FilteredGlobalUsers { get; private set; } = new ObservableCollection<UserLite>();

        public event PropertyChangedEventHandler PropertyChanged;

        // ---------- Friends (min venneliste) ----------
        public async Task LoadFriendsAsync()
        {
            var list = await SafeGetFriendsAsync();
            Friends.Clear();
            for (int i = 0; i < list.Count; i++)
                Friends.Add( list[ i ] );
            OnPropertyChanged( "Friends" );
        }

        public async Task RefreshFriendsIfEmptyAsync()
        {
            if (Friends.Count == 0)
                await LoadFriendsAsync();
        }

        public async Task<bool> AddFriendAsync( UserLite user )
        {
            if (user == null) return false;

            var ok = await _api.AddFriendAsync( user.UserId );
            if (ok)
            {
                // undgå dubletter
                if (!Friends.Any( f => f.UserId == user.UserId ))
                    Friends.Insert( 0, user );
                OnPropertyChanged( "Friends" );
            }
            return ok;
        }

        public async Task<bool> RemoveFriendAsync( UserLite user )
        {
            if (user == null) return false;

            var ok = await _api.RemoveFriendAsync( user.UserId );
            if (ok)
            {
                var idx = IndexOf( Friends, user.UserId );
                if (idx >= 0) Friends.RemoveAt( idx );
                OnPropertyChanged( "Friends" );
            }
            return ok;
        }

        // ---------- Global liste ("Alle"-fanen) ----------
        public async Task LoadAllUsersAsync()
        {
            var list = await SafeGetAllUsersAsync();
            GlobalUsers.Clear();
            for (int i = 0; i < list.Count; i++)
                GlobalUsers.Add( list[ i ] );

            // start med at vise alle
            FilterGlobal( string.Empty );
        }

        /// <summary>
        /// Client-side filter af GlobalUsers. Hvis query &lt; 2 tegn → vis alle.
        /// </summary>
        public void FilterGlobal( string query )
        {
            query = (query ?? string.Empty).Trim();

            IEnumerable<UserLite> src = GlobalUsers;
            if (query.Length >= 2)
            {
                src = src.Where( u =>
                    SafeContains( u.FullName, query ) ||
                    SafeContains( u.UserName, query ) ||
                    SafeContains( u.EmailFallback(), query ) );
            }

            FilteredGlobalUsers.Clear();
            foreach (var u in src)
                FilteredGlobalUsers.Add( u );

            OnPropertyChanged( "FilteredGlobalUsers" );
        }

        // ---------- Hjælpere ----------
        private async Task<IReadOnlyList<UserLite>> SafeGetFriendsAsync()
        {
            try
            {
                var list = await _api.GetFriendsAsync();
                return list ?? new List<UserLite>();
            }
            catch
            {
                return new List<UserLite>();
            }
        }

        private async Task<IReadOnlyList<UserLite>> SafeGetAllUsersAsync()
        {
            try
            {
                var list = await _api.GetAllUsersAsync();
                return list ?? new List<UserLite>();
            }
            catch
            {
                return new List<UserLite>();
            }
        }

        private static bool SafeContains( string value, string query )
        {
            if (string.IsNullOrEmpty( value ) || string.IsNullOrEmpty( query ))
                return false;
            return value.IndexOf( query, StringComparison.OrdinalIgnoreCase ) >= 0;
        }

        private static int IndexOf( ObservableCollection<UserLite> list, int userId )
        {
            for (int i = 0; i < list.Count; i++)
                if (list[ i ] != null && list[ i ].UserId == userId)
                    return i;
            return -1;
        }

        private void OnPropertyChanged( [CallerMemberName] string name = null )
        {
            var h = PropertyChanged;
            if (h != null) h( this, new PropertyChangedEventArgs( name ) );
        }
    }

    // Lille extension til at have et fallback-felt at søge i, hvis API ikke sender UserName
    internal static class UserLiteExtensions
    {
        public static string EmailFallback( this UserLite u )
        {
            // nogle API'er returnerer Email i UserName – ellers returnér tom
            return u == null ? "" : (u.UserName ?? "");
        }
    }
}
