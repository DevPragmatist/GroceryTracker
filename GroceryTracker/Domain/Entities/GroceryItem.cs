using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryTracker.Domain.Entities;

public class GroceryItem : Entity
{
    public required string Name { get; set; }
    public decimal Size { get; set; }
    public decimal Price { get; set; }
    public required Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public List<PriceHistory> PriceHistories { get; set; } = [];
    
}

public class GroceryItemEntityTypeConfiguration : IEntityTypeConfiguration<GroceryItem>
{
    public void Configure(EntityTypeBuilder<GroceryItem> builder)
    {
        builder.Property(gi => gi.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(gi => gi.Size)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        builder.Property(gi => gi.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        builder.HasOne(gi => gi.Category)
            .WithMany(c => c.GroceryItems)
            .HasForeignKey(gi => gi.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
