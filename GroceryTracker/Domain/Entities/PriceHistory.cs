using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryTracker.Domain.Entities;

public class PriceHistory : Entity
{
    public Guid GroceryItemId { get; set; }
    public required GroceryItem GroceryItem { get; set; }
    public required Store Store { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
}

public class PriceHistoryEntityTypeConfiguration : IEntityTypeConfiguration<PriceHistory>
{
    public void Configure(EntityTypeBuilder<PriceHistory> builder)
    {
        builder.HasOne(ph => ph.GroceryItem)
            .WithMany(gi => gi.PriceHistories)
            .HasForeignKey(ph => ph.GroceryItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ph => ph.Store)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(ph => ph.OldPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        builder.Property(ph => ph.NewPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
    }
}