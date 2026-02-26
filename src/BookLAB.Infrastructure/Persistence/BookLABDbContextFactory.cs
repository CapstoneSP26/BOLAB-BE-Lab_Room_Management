using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookLAB.Infrastructure.Persistence
{
    public class BookLABDbContextFactory
        : IDesignTimeDbContextFactory<BookLABDbContext>
    {
        public BookLABDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BookLABDbContext>();

            optionsBuilder.UseNpgsql(
                "Server=bolab-database.postgres.database.azure.com;Database=postgres;Port=5432;User Id=pgadmin;Password=bolabdb@123;Ssl Mode=Require;");

            return new BookLABDbContext(optionsBuilder.Options);
        }
    }
}