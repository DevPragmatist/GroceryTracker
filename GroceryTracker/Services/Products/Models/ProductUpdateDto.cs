namespace GroceryTracker.Services.Products.Models;

public record ProductUpdateDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required decimal Size { get; set; }
    public required string SizeUnit { get; set; }
    public required decimal Price { get; set; }
    public Guid? StoreId { get; set; }
    public required string StoreName { get; set; }
    public required string Location { get; set; }
    public required DateTime PurchaseDate { get; set; }
}
