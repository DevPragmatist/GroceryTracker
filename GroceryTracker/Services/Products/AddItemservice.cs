using GroceryTracker.Domain;
using GroceryTracker.Domain.Entities;
using GroceryTracker.Services.Products.Models;

namespace GroceryTracker.Services.Products;

public interface IAddItemService
{
    Task AddNewItem(ProductAddDto product);
}

public class AddItemService : IAddItemService
{
    private readonly IUnitOfWork _unitOfWork;

    public AddItemService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddNewItem(ProductAddDto product)
    {
        var category = _unitOfWork.Set<Category>().Where(c => c.Id == product.CategoryId).FirstOrDefault();
        if(category is null)
        {
            category = new Category
            {
                Id = Guid.NewGuid(),
                Name = product.CategoryName
            };
            _unitOfWork.Set<Category>().Add(category);
        }
        var store = _unitOfWork.Set<Store>().Where(s => s.Id == product.StoreId).FirstOrDefault();
        if(store is null)
        {
            store = new Store
            {
                Id = Guid.NewGuid(),
                Name = product.StoreName,
                Location = product.Location
            };
            _unitOfWork.Set<Store>().Add(store);
        }
        var newItem = new Product
        {
            Id = Guid.NewGuid(),
            Name = product.Name,
            Size = product.Size,
            Price = product.Price,
            CategoryId = category.Id,
            PriceHistories =
            [
                new() {
                    Id = Guid.NewGuid(),
                    NewPrice = product.Price,
                    StoreId = store.Id,
                    PurchaseDate = product.PurchaseDate
                }
            ]
        };

        store.Products.Add(newItem);

        await _unitOfWork.SaveChangesAsync();
    }
}
