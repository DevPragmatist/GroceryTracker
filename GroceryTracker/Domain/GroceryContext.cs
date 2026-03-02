using GroceryTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GroceryTracker.Domain;

public class GroceryContext : DbContext, IUnitOfWork
{
    public GroceryContext(DbContextOptions<GroceryContext> options) : base(options)
    {
    }
    public DbSet<GroceryItem> GroceryItems { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<PriceHistory> PriceHistories { get; set; }

    public DbContext Context => this;

    IQueryable<T> IUnitOfWork.Query<T>() where T : class => Set<T>().AsQueryable().AsNoTracking();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GroceryContext).Assembly);

        var entities = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(Entity).IsAssignableFrom(t.ClrType))
            .Select(x => modelBuilder.Entity(x.ClrType));

        foreach (var e in entities)
        {
            e.HasKey(nameof(Entity.Id));
            e.Property(nameof(Entity.DateCreated)).HasColumnType("datetimeoffset").IsRequired();
        }
    }
}
