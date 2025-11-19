using BatchPay.Models;
using System.Text.Json;

namespace FrontendServices
{
    public class BatchPayService : IBatchPayService
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions JsonOpts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public BatchPayService()
        {
#if ANDROID
    var baseUrl = "http://10.0.2.2:5000/";
    _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
#else
            _http = new HttpClient { BaseAddress = new Uri( "http://localhost:5000/" ) };
#endif

            System.Diagnostics.Debug.WriteLine( $"[BatchPayService] BaseAddress = {_http.BaseAddress}" );
            _http.DefaultRequestHeaders.Remove( "X-UserId" );
            _http.DefaultRequestHeaders.Add( "X-UserId", "13" );

        }

        // Brug helper så vi ser status/body hvis noget fejler
        private static async Task<T> GetJson<T>( HttpClient http, string url )
        {
            var resp = await http.GetAsync( url );
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException( $"GET {http.BaseAddress}{url} -> {(int)resp.StatusCode} {resp.ReasonPhrase}\n{body}" );
            return JsonSerializer.Deserialize<T>( body, JsonOpts );
        }

        public Task<IReadOnlyList<UserLite>> GetAllUsersAsync()
            => GetJson<IReadOnlyList<UserLite>>( _http, "api/users" );

        public Task<IReadOnlyList<UserLite>> SearchUsersAsync( string q )
            => GetJson<IReadOnlyList<UserLite>>( _http, "api/users/search?q=" + Uri.EscapeDataString( q ?? "" ) );

        public Task<IReadOnlyList<UserLite>> GetFriendsAsync()
            => GetJson<IReadOnlyList<UserLite>>( _http, "api/friends" );

        public async Task<bool> AddFriendAsync( int userId )
        {
            var resp = await _http.PostAsync( $"api/friends/{userId}", null );
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                throw new HttpRequestException( $"POST {_http.BaseAddress}api/friends/{userId} -> {(int)resp.StatusCode} {resp.ReasonPhrase}\n{body}" );
            }
            return true;
        }

        public async Task<bool> RemoveFriendAsync( int userId )
        {
            var resp = await _http.DeleteAsync( $"api/friends/{userId}" );
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                throw new HttpRequestException( $"DELETE {_http.BaseAddress}api/friends/{userId} -> {(int)resp.StatusCode} {resp.ReasonPhrase}\n{body}" );
            }
            return true;
        }
    }
}
