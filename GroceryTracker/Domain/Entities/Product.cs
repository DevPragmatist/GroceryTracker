using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryTracker.Domain.Entities;

public class Product : Entity
{
    public required string Name { get; set; }
    public decimal Size { get; set; }
    public decimal Price { get; set; }
    public required Guid CategoryId { get; set; }
    public Category? Category { get; set; } = null!;
    public List<PriceHistory> PriceHistories { get; set; } = [];
    
}

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(p => p.Size)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
