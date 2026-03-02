using Microsoft.EntityFrameworkCore;

namespace GroceryTracker.Domain;

public interface IUnitOfWork
{
    DbSet<T> Set<T>() where T : class;
    IQueryable<T> Query<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    DbContext Context { get; }
}
