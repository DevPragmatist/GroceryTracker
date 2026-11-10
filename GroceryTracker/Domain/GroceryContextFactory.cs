using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GroceryTracker.Domain;

public sealed class GroceryContextFactory : IDesignTimeDbContextFactory<GroceryContext>
{
    public GroceryContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__GroceryContext")
            ?? "Server=localhost;Database=GroceryTrackerDb;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=false";

        var optionsBuilder = new DbContextOptionsBuilder<GroceryContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new GroceryContext(optionsBuilder.Options);
    }
}