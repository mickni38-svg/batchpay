using BatchPay.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data.infrastructure
{
    public class BatchPayContextFactory : IDesignTimeDbContextFactory<BatchPayContext>
    {
        public BatchPayContext CreateDbContext( string[] args )
        {
            var optionsBuilder = new DbContextOptionsBuilder<BatchPayContext>();

            // Brug din SQL Express connection string
            optionsBuilder.UseSqlServer(
                @"Server=DESKTOP-HNI6DDI\SQLEXPRESS;Database=BatchPay;Trusted_Connection=True;TrustServerCertificate=True;");


            return new BatchPayContext( optionsBuilder.Options );
        }
    }
}
