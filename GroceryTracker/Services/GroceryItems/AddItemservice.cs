using GroceryTracker.Domain;
using GroceryTracker.Domain.Entities;
using GroceryTracker.Services.GroceryItems.Models;

namespace GroceryTracker.Services.GroceryItems;

public interface IAddItemService
{
    Task AddNewItem(GroceryItemAddDto groceryItem);
}

public class AddItemService : IAddItemService
{
    private readonly IUnitOfWork _unitOfWork;

    public AddItemService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddNewItem(GroceryItemAddDto groceryItem)
    {
        var store = _unitOfWork.Set<Store>().Where(s => s.Id == groceryItem.StoreId).FirstOrDefault() ?? throw new Exception("Store not found");

        var newItem = new GroceryItem
        {
            Id = Guid.NewGuid(),
            Name = groceryItem.Name,
            Size = groceryItem.Size,
            Price = groceryItem.Price,
            CategoryId = groceryItem.CategoryId
        };

        store.GroceryItems.Add(newItem);

        await _unitOfWork.SaveChangesAsync();
    }
}
