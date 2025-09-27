using Microsoft.EntityFrameworkCore;
using BatchPay.Models;

namespace BatchPay.Data
{
    public class BatchPayContext : DbContext
    {
        public BatchPayContext( DbContextOptions<BatchPayContext> options ) : base( options )
        {
        }

        // Parameterløs constructor til migrations
        public BatchPayContext()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderParticipant> OrderParticipants { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }

        protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=DESKTOP-HNI6DDI\\SQLEXPRESS;Database=BatchPay;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;" );



            }
        }

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            base.OnModelCreating( modelBuilder );

            // Unikt OrderCode
            modelBuilder.Entity<Order>()
                .HasIndex( o => o.OrderCode )
                .IsUnique();

            // Relationer: Order -> CreatedBy
            modelBuilder.Entity<Order>()
                .HasOne( o => o.CreatedBy )
                .WithMany()
                .HasForeignKey( o => o.CreatedByUserId )
                .OnDelete( DeleteBehavior.Restrict );

            // Relationer: OrderParticipant -> User
            modelBuilder.Entity<OrderParticipant>()
                .HasOne( op => op.User )
                .WithMany( u => u.OrderParticipants )
                .HasForeignKey( op => op.UserId );

            // Relationer: OrderParticipant -> Order
            modelBuilder.Entity<OrderParticipant>()
                .HasOne( op => op.Order )
                .WithMany( o => o.Participants )
                .HasForeignKey( op => op.OrderId );

            // Relationer: OrderLine -> User
            modelBuilder.Entity<OrderLine>()
                .HasOne( ol => ol.User )
                .WithMany( u => u.OrderLines )
                .HasForeignKey( ol => ol.UserId );

            // Relationer: OrderLine -> Order
            modelBuilder.Entity<OrderLine>()
                .HasOne( ol => ol.Order )
                .WithMany( o => o.OrderLines )
                .HasForeignKey( ol => ol.OrderId );

            // Kolonne-definition for decimal (penge)
            modelBuilder.Entity<OrderLine>()
                .Property( ol => ol.Price )
                .HasColumnType( "decimal(18,2)" );
        }
    }
}
