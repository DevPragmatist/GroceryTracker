using GroceryTracker.Domain;
using GroceryTracker.Domain.Entities;
using GroceryTracker.Services.Products.Models;

namespace GroceryTracker.Services.Products;

public interface IUpdateItemService
{
    Task UpdateItem(ProductUpdateDto updateDto);
}

public class UpdateItemService : IUpdateItemService
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateItemService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task UpdateItem(ProductUpdateDto updateDto)
    {
        var item = _unitOfWork.Set<Product>().FirstOrDefault(i => i.Id == updateDto.Id) ?? throw new Exception("Item not found");
        var store = _unitOfWork.Set<Store>().Where(s => s.Id == updateDto.StoreId).FirstOrDefault();
        if(store is null)
        {
            store = new Store
            {
                Id = Guid.NewGuid(),
                Name = updateDto.StoreName,
                Location = updateDto.Location
            };
            _unitOfWork.Set<Store>().Add(store);
        }
        if (item.Price != updateDto.Price)
        {
            item.PriceHistories.Add(new PriceHistory
            {
                Id = Guid.NewGuid(),
                ProductId = item.Id,
                OldPrice = item.Price,
                NewPrice = updateDto.Price,
                StoreId = store.Id,
                PurchaseDate = updateDto.PurchaseDate
            });
        }
        item.Name = updateDto.Name;
        item.Size = updateDto.Size;
        item.Price = updateDto.Price;

        await _unitOfWork.SaveChangesAsync();
    }
}