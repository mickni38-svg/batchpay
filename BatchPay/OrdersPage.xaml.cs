using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Maui;
using System.Linq.Expressions;
namespace BatchPay
{
    public partial class OrdersPage : ContentPage
    {
        public ObservableCollection<OrderDto> Orders { get; set; }

        public OrdersPage()
        {
            InitializeComponent();
            Orders = new ObservableCollection<OrderDto>();
            LoadOrdersFromApi();
            BindingContext = this;
        }

        private async void LoadOrdersFromApi()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync( "http://10.0.2.2:5000/api/orders" );


            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var orders = System.Text.Json.JsonSerializer.Deserialize<List<OrderDto>>( json );

                Orders.Clear();
                foreach (var order in orders)
                    Orders.Add( order );
            }
        }

        private async void OnTestApiClicked( object sender, EventArgs e )
        {
            try
            {
                using var client = new HttpClient();

                // Brug http og ikke https, ellers skal emulatoren have certifikat
                var response = await client.GetAsync( "http://10.0.2.2:5000/api/orders" );

                await DisplayAlert( "API Test",
                    $"Status code: {response.StatusCode}",
                    "OK" );
            }
            catch (Exception ex)
            {
                await DisplayAlert( "API Test Error",
                    ex.Message,
                    "OK" );
            }
        }


    }

    public class OrderDto
    {
        public string Title { get; set; }
        public string Status { get; set; }
        public List<OrderLineDto> OrderLines { get; set; }
        public List<ParticipantDto> Participants { get; set; }
    }

    public class OrderLineDto
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ParticipantDto
    {
        public string UserName { get; set; }
        public bool HasPaid { get; set; }
    }
}
