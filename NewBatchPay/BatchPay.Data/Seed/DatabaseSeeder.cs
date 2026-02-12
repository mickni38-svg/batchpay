using BatchPay.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Data.Seed;

public sealed class DatabaseSeeder( BatchPayContext db )
{
    public async Task SeedMerchantsAsync( CancellationToken ct )
    {
        var merchants = new[]
        {
            new
            {
                DisplayName = "Sticks & Sushi",
                Handle = "sticksandsushi",
                Website = "https://sticksandsushi.com",
                ContactEmail = "kontakt@sticksandsushi.com",
                City = "København"
            },
            new
            {
                DisplayName = "Gasoline Grill",
                Handle = "gasolinegrill",
                Website = "https://gasolinegrill.com",
                ContactEmail = "kontakt@gasolinegrill.com",
                City = "København"
            }
        };

        var existingHandles = await db.Merchants
            .Select( m => m.Handle )
            .ToListAsync( ct );

        var existingSet = new HashSet<string>(
            existingHandles,
            StringComparer.OrdinalIgnoreCase
        );

        foreach (var m in merchants)
        {
            if (existingSet.Contains( m.Handle ))
                continue;

            var merchant = new MerchantEntity
            {
                DisplayName = m.DisplayName,
                Handle = m.Handle,
                WebsiteUrl = m.Website,
                ContactEmail = m.ContactEmail,
                City = m.City,
                CountryCode = "DK",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            db.Merchants.Add( merchant );
            await db.SaveChangesAsync( ct ); // så vi får merchant.Id

            var integration = new MerchantIntegrationEntity
            {
                MerchantId = merchant.Id,
                WebhookUrl = $"https://example.com/webhooks/sbys/{merchant.Handle}",
                DefaultReturnUrl = merchant.WebsiteUrl,
                AllowedOrigin = merchant.WebsiteUrl,
                ApiKeyHash = $"DEV_API_KEY_{merchant.Handle.ToUpperInvariant()}",
                SigningSecretHash = $"DEV_SECRET_{merchant.Handle.ToUpperInvariant()}",
                IsEnabled = true,
                UpdatedAtUtc = DateTime.UtcNow
            };

            db.MerchantIntegrations.Add( integration );
        }

        await db.SaveChangesAsync( ct );
    }

    public async Task SeedUsersAsync( CancellationToken ct )
    {
        var seed = new (string name, string handle)[]
        {
            ("Mads Grønlund", "mads"),
            ("Nikolaj Holm", "nikolaj"),
            ("Steff Warto", "steff"),
            ("Adda Algren", "adda"),
            ("Sofie Lund", "sofie"),
            ("Emil Jensen", "emil"),
            ("Sara Nørgaard", "sara"),
            ("Oliver Dahl", "oliver"),
            ("Freja Hansen", "freja"),
            ("Noah Pedersen", "noah"),
            ("Alma Kristensen", "alma"),
            ("Victor Møller", "victor"),
            ("Laura Nielsen", "laura"),
            ("Lucas Rasmussen", "lucas"),
            ("Anna Mortensen", "anna"),
            ("Oscar Sørensen", "oscar"),
            ("Clara Thomsen", "clara"),
            ("Malthe Eriksen", "malthe"),
            ("Ida Andersen", "ida"),
            ("William Skov", "william"),
        };

        var existing = await db.Users.Select( u => u.Handle ).ToListAsync( ct );
        var set = new HashSet<string>( existing, StringComparer.OrdinalIgnoreCase );

        foreach (var (name, handle) in seed)
        {
            if (set.Contains( handle )) continue;

            db.Users.Add( new Entities.UserEntity
            {
                DisplayName = name,
                Handle = handle
            } );
        }

        await db.SaveChangesAsync( ct );
    }
}
