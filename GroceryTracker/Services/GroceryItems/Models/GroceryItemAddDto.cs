namespace GroceryTracker.Services.GroceryItems.Models;

public record GroceryItemAddDto
{
    public required Guid StoreId { get; set; }
    public required string Name { get; set; }
    public required decimal Size { get; set; }
    public required decimal Price { get; set; }
    public required Guid CategoryId { get; set; }
}
