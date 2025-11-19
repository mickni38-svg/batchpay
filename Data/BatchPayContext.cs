using Microsoft.EntityFrameworkCore;
using Data.Model;

namespace Data
{
    // DbContext for BatchPay – mapper til eksisterende tabeller dbo.Users og dbo.FriendRequests
    public class BatchPayContext : DbContext
    {
        public BatchPayContext( DbContextOptions<BatchPayContext> options ) : base( options ) { }

        public DbSet<User> Users { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }

        protected override void OnModelCreating( ModelBuilder b )
        {
            base.OnModelCreating( b );

            // ---------- Users ----------
            b.Entity<User>( e =>
            {
                e.ToTable( "Users" );
                e.HasKey( x => x.UserId );

                e.Property( x => x.Name ).IsRequired();
                e.Property( x => x.Email ).IsRequired();

                e.HasIndex( x => x.Email ).IsUnique();
            } );

            // ---------- FriendRequests ----------
            b.Entity<FriendRequest>( e =>
            {
                e.ToTable( "FriendRequests" );
                e.HasKey( x => x.FriendRequestId );
                e.Property( x => x.Status ).IsRequired();

                // FJERNET:
                // e.Property(x => x.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");

                e.HasOne( x => x.Requester )
                 .WithMany( u => u.SentFriendRequests )
                 .HasForeignKey( x => x.RequesterId )
                 .OnDelete( DeleteBehavior.Restrict );

                e.HasOne( x => x.Receiver )
                 .WithMany( u => u.ReceivedFriendRequests )
                 .HasForeignKey( x => x.ReceiverId )
                 .OnDelete( DeleteBehavior.Restrict );

                e.HasIndex( x => new { x.RequesterId, x.ReceiverId } ).IsUnique();
            } );
        
        }
   
    }
}
