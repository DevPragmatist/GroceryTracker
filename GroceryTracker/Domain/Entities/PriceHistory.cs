using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryTracker.Domain.Entities;

public class PriceHistory : Entity
{
    public Guid ProductId { get; set; }
    public  Product Product { get; set; } = null!;
    public required Guid StoreId { get; set; }
    public  Store Store { get; set; } = null!;
    public decimal? OldPrice { get; set; }
    public required decimal NewPrice { get; set; }
    public required DateTime PurchaseDate { get; set; }
}

public class PriceHistoryEntityTypeConfiguration : IEntityTypeConfiguration<PriceHistory>
{
    public void Configure(EntityTypeBuilder<PriceHistory> builder)
    {
        builder.HasOne(ph => ph.Product)
            .WithMany(p => p.PriceHistories)
            .HasForeignKey(ph => ph.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
            

        builder.HasOne(ph => ph.Store)
            .WithMany()
            .HasForeignKey(ph => ph.StoreId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(ph => ph.OldPrice)
            .IsRequired(false)
            .HasColumnType("decimal(18,2)");
        
        builder.Property(ph => ph.NewPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        builder.Property(ph => ph.PurchaseDate)
            .IsRequired();
    }
}