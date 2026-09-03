namespace GroceryTracker.Services.Products.Models;

public class UpsertProductRequestDto
{
    public Guid? ProductId{get; set;}
    public required string Name { get; set; }
    public Guid? StoreId { get; set; }
    public required string StoreName { get; set; }
    public required decimal Size { get; set; }
    public required string SizeUnit { get; set; }
    public required decimal Price { get; set; }
    public required string Location { get; set; }
    public required DateTime PurchaseDate { get; set; }
    public Guid? CategoryId { get; set; }
    public required string CategoryName { get; set; }
}
