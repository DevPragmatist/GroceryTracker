using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryTracker.Domain.Entities;

public class Store : Entity
{
    public required string Name { get; set; }
    public required string Location { get; set; }
    public List<GroceryItem> GroceryItems { get; set; } = [];
}

public class StoreEntityTypeConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(s => s.Location)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.HasMany(s => s.GroceryItems)
            .WithOne()
            .OnDelete(DeleteBehavior.SetNull);
    }
}