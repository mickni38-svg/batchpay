namespace BatchPay.Models
{
    public sealed class MerchantModel
    {
        // Det du sender til backend senere (MerchantId)
        public string Id { get; set; } = string.Empty;

        // Det brugeren ser i listen
        public string Name { get; set; } = string.Empty;

        // Valgfrit – nice to have i UI
        public string? Category { get; set; }     // fx "Pizza", "Burger"
        public string? City { get; set; }         // fx "København"
        public bool HasSubscription { get; set; } = true; // kun dem med abonnement vises
    }
}
