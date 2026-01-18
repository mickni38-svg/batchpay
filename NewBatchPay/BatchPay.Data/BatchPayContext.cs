using BatchPay.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Data;

public sealed class BatchPayContext( DbContextOptions<BatchPayContext> options ) : DbContext( options )
{
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<FriendRequestEntity> FriendRequests => Set<FriendRequestEntity>();
    public DbSet<GroupPaymentEntity> GroupPayments => Set<GroupPaymentEntity>();
    public DbSet<GroupPaymentMemberEntity> GroupPaymentMembers => Set<GroupPaymentMemberEntity>();

    // NEW:
    public DbSet<MerchantEntity> Merchants => Set<MerchantEntity>();
    public DbSet<MerchantIntegrationEntity> MerchantIntegrations => Set<MerchantIntegrationEntity>();
    public DbSet<DirectoryConnectionEntity> DirectoryConnections => Set<DirectoryConnectionEntity>();

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<UserEntity>( e =>
        {
            e.ToTable( "Users", "dbo" );
            e.HasKey( x => x.Id );
            e.Property( x => x.DisplayName ).HasMaxLength( 100 ).IsRequired();
            e.Property( x => x.Handle ).HasMaxLength( 50 ).IsRequired();
            e.HasIndex( x => x.Handle ).IsUnique();
        } );

        modelBuilder.Entity<FriendRequestEntity>( e =>
        {
            e.ToTable( "FriendRequest", "dbo" );
            e.HasKey( x => x.Id );

            e.HasIndex( x => new { x.RequesterUserId, x.ReceiverUserId } ).IsUnique();

            e.HasOne( x => x.Requester )
                .WithMany()
                .HasForeignKey( x => x.RequesterUserId )
                .OnDelete( DeleteBehavior.Restrict );

            e.HasOne( x => x.Receiver )
                .WithMany()
                .HasForeignKey( x => x.ReceiverUserId )
                .OnDelete( DeleteBehavior.Restrict );
        } );

        modelBuilder.Entity<GroupPaymentEntity>( e =>
        {
            e.ToTable( "GroupPayments", "dbo" );
            e.HasKey( x => x.Id );
            e.Property( x => x.Title ).HasMaxLength( 120 ).IsRequired();
            e.Property( x => x.Message ).HasMaxLength( 500 );
            e.Property( x => x.IconKey ).HasMaxLength( 50 );

            // ✅ Soft delete (shadow props)
            e.Property<bool>( "IsActive" ).HasDefaultValue( true );
            e.Property<DateTime?>( "DeactivatedAtUtc" );

            e.HasOne( x => x.CreatedBy )
                .WithMany()
                .HasForeignKey( x => x.CreatedByUserId )
                .OnDelete( DeleteBehavior.Restrict );
        } );

        modelBuilder.Entity<GroupPaymentMemberEntity>( e =>
        {
            e.ToTable( "GroupPaymentMembers", "dbo" );
            e.HasKey( x => x.Id );

            e.HasIndex( x => new { x.GroupPaymentId, x.UserId } ).IsUnique();

            // ✅ Soft delete (shadow props)
            e.Property<bool>( "IsActive" ).HasDefaultValue( true );
            e.Property<DateTime?>( "DeactivatedAtUtc" );

            e.HasOne( x => x.GroupPayment )
                .WithMany( x => x.Members )
                .HasForeignKey( x => x.GroupPaymentId );

            e.HasOne( x => x.User )
                .WithMany()
                .HasForeignKey( x => x.UserId )
                .OnDelete( DeleteBehavior.Restrict );
        } );

        // -------------------------
        // NEW: Merchants
        // -------------------------
        modelBuilder.Entity<MerchantEntity>( e =>
        {
            e.ToTable( "Merchants", "dbo" );
            e.HasKey( x => x.Id );

            e.Property( x => x.DisplayName ).HasMaxLength( 150 ).IsRequired();
            e.Property( x => x.Handle ).HasMaxLength( 50 ).IsRequired();
            e.HasIndex( x => x.Handle ).IsUnique();

            e.Property( x => x.Description ).HasMaxLength( 500 );
            e.Property( x => x.LogoUrl ).HasMaxLength( 500 );
            e.Property( x => x.WebsiteUrl ).HasMaxLength( 500 );

            e.Property( x => x.ContactEmail ).HasMaxLength( 200 ).IsRequired();
            e.Property( x => x.ContactPhone ).HasMaxLength( 50 );

            e.Property( x => x.AddressLine1 ).HasMaxLength( 200 );
            e.Property( x => x.City ).HasMaxLength( 100 );
            e.Property( x => x.PostalCode ).HasMaxLength( 20 );
            e.Property( x => x.CountryCode ).HasMaxLength( 2 );

            e.Property( x => x.IsActive ).HasDefaultValue( true );
        } );

        modelBuilder.Entity<MerchantIntegrationEntity>( e =>
        {
            e.ToTable( "MerchantIntegrations", "dbo" );

            // 1:1 (MerchantId er både PK og FK)
            e.HasKey( x => x.MerchantId );

            e.Property( x => x.WebhookUrl ).HasMaxLength( 500 ).IsRequired();
            e.Property( x => x.DefaultReturnUrl ).HasMaxLength( 500 );
            e.Property( x => x.AllowedOrigin ).HasMaxLength( 200 );

            e.Property( x => x.ApiKeyHash ).HasMaxLength( 200 ).IsRequired();
            e.Property( x => x.SigningSecretHash ).HasMaxLength( 200 ).IsRequired();

            e.Property( x => x.IsEnabled ).HasDefaultValue( true );

            e.HasOne( x => x.Merchant )
                .WithOne( m => m.Integration )
                .HasForeignKey<MerchantIntegrationEntity>( x => x.MerchantId )
                .OnDelete( DeleteBehavior.Cascade );
        } );

        // -------------------------
        // NEW: Directory connections (User -> User/Merchant)
        // -------------------------
        modelBuilder.Entity<DirectoryConnectionEntity>( e =>
        {
            e.ToTable( "DirectoryConnections", "dbo" );
            e.HasKey( x => x.Id );

            e.Property( x => x.TargetType ).IsRequired();
            e.Property( x => x.IsActive ).HasDefaultValue( true );

            // Unique: samme owner må kun have én aktiv forbindelse til samme target
            e.HasIndex( x => new { x.OwnerUserId, x.TargetType, x.TargetId } ).IsUnique();

            e.HasOne( x => x.OwnerUser )
                .WithMany()
                .HasForeignKey( x => x.OwnerUserId )
                .OnDelete( DeleteBehavior.Restrict );
        } );
    }
}
