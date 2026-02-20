using BatchPay.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchPay.Data;

public sealed class BatchPayContext( DbContextOptions<BatchPayContext> options ) : DbContext( options )
{
    // Unified DbSet for both Users and Merchants
    public DbSet<DirectoryEntryEntity> DirectoryEntries => Set<DirectoryEntryEntity>();

    // Other DbSets
    public DbSet<FriendRequestEntity> FriendRequests => Set<FriendRequestEntity>();
    public DbSet<GroupPaymentEntity> GroupPayments => Set<GroupPaymentEntity>();
    public DbSet<GroupPaymentMemberEntity> GroupPaymentMembers => Set<GroupPaymentMemberEntity>();
    public DbSet<MerchantIntegrationEntity> MerchantIntegrations => Set<MerchantIntegrationEntity>();

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        // 1. TPH Configuration for DirectoryEntry (The Root)
        modelBuilder.Entity<DirectoryEntryEntity>()
            .ToTable( "DirectoryEntries" )
            .HasDiscriminator<string>( "EntryType" )
            .HasValue<UserEntity>( nameof( UserEntity ) )
            .HasValue<MerchantEntity>( nameof( MerchantEntity ) );

        modelBuilder.Entity<DirectoryEntryEntity>( e =>
        {
            e.HasKey( x => x.Id );
            e.Property( x => x.DisplayName ).HasMaxLength( 150 ).IsRequired();
            e.Property( x => x.Handle ).HasMaxLength( 50 ).IsRequired();
            e.HasIndex( x => x.Handle ).IsUnique();
            e.Property( x => x.IsActive ).HasDefaultValue( true );
        } );

        // 2. Configuration for UserEntity (Derived Type)
        modelBuilder.Entity<UserEntity>( e =>
        {
            // Adds the 'Email' column to the 'DirectoryEntries' table for User rows
            e.Property( x => x.Email ).HasMaxLength( 200 ).IsRequired();
        } );

        // 3. Configuration for MerchantEntity (Derived Type)
        modelBuilder.Entity<MerchantEntity>( e =>
        {
            // Adds merchant-specific columns to the 'DirectoryEntries' table for Merchant rows
            e.Property( x => x.Description ).HasMaxLength( 500 );
            e.Property( x => x.WebsiteUrl ).HasMaxLength( 500 );
            e.Property( x => x.ContactEmail ).HasMaxLength( 200 ).IsRequired();
            e.Property( x => x.LogoUrl ).HasMaxLength( 500 );
            e.Property( x => x.ContactPhone ).HasMaxLength( 50 );
            e.Property( x => x.AddressLine1 ).HasMaxLength( 200 );
            e.Property( x => x.City ).HasMaxLength( 100 );
            e.Property( x => x.PostalCode ).HasMaxLength( 20 );
            e.Property( x => x.CountryCode ).HasMaxLength( 2 );
        } );

        // 4. Relationship Configurations
        modelBuilder.Entity<FriendRequestEntity>( e =>
        {
            e.ToTable( "FriendRequests" );
            e.HasKey( x => x.Id );
            e.HasIndex( x => new { x.RequesterId, x.ReceiverId } ).IsUnique();

            e.HasOne( x => x.Requester )
                .WithMany()
                .HasForeignKey( x => x.RequesterId )
                .HasConstraintName( "FK_FriendRequests_DirectoryEntries_RequesterId" )
                .OnDelete( DeleteBehavior.Restrict );

            e.HasOne( x => x.Receiver )
                .WithMany()
                .HasForeignKey( x => x.ReceiverId )
                .HasConstraintName( "FK_FriendRequests_DirectoryEntries_ReceiverId" )
                .OnDelete( DeleteBehavior.Restrict );
        } );


        modelBuilder.Entity<GroupPaymentEntity>( e =>
        {
            e.ToTable( "GroupPayments" );
            e.HasKey( x => x.Id );
            e.Property( x => x.Title ).HasMaxLength( 120 ).IsRequired();
            e.Property( x => x.Message ).HasMaxLength( 500 );
            e.Property( x => x.IconKey ).HasMaxLength( 50 );

            e.HasOne( x => x.CreatedBy )
                .WithMany()
                .HasForeignKey( x => x.CreatedByUserId )
                .OnDelete( DeleteBehavior.Restrict );
        } );

        modelBuilder.Entity<GroupPaymentMemberEntity>( e =>
        {
            e.ToTable( "GroupPaymentMembers" );
            e.HasKey( x => x.Id );
            e.HasIndex( x => new { x.GroupPaymentId, x.MemberId } ).IsUnique();

            e.HasOne( x => x.GroupPayment )
                .WithMany( x => x.Members )
                .HasForeignKey( x => x.GroupPaymentId );

            e.HasOne( x => x.Member )
                .WithMany()
                .HasForeignKey( x => x.MemberId )
                .OnDelete( DeleteBehavior.Restrict );
        } );


        modelBuilder.Entity<MerchantIntegrationEntity>( e =>
        {
            e.ToTable( "MerchantIntegrations" );
            e.HasKey( x => x.MerchantId );

            e.HasOne( x => x.Merchant )
                .WithOne( m => m.Integration )
                .HasForeignKey<MerchantIntegrationEntity>( x => x.MerchantId )
                .OnDelete( DeleteBehavior.Cascade );
        } );
    }
}
