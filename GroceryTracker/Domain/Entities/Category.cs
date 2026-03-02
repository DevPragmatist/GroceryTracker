using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryTracker.Domain.Entities;

public class Category : Entity
{
    public required string Name { get; set; }
    public required string Description { get; set; }

    public List<GroceryItem> GroceryItems { get; set; } = [];
}

public sealed class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder){
        builder.HasMany(c => c.GroceryItems)
            .WithOne(gi => gi.Category)
            .HasForeignKey(gi => gi.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(500);
    }
}
