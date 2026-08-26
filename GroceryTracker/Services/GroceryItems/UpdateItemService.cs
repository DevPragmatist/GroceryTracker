using GroceryTracker.Domain;
using GroceryTracker.Domain.Entities;
using GroceryTracker.Services.GroceryItems.Models;

namespace GroceryTracker.Services.GroceryItems;

public interface IUpdateItemService
{
    Task UpdateItem(GroceryItemUpdateDto updateDto);
}

public class UpdateItemService : IUpdateItemService
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateItemService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task UpdateItem(GroceryItemUpdateDto updateDto)
    {
        var item = _unitOfWork.Set<GroceryItem>().FirstOrDefault(i => i.Id == updateDto.Id) ?? throw new Exception("Item not found");

        if (item.Price != updateDto.Price)
        {
            item.PriceHistories.Add(new PriceHistory
            {
                Id = Guid.NewGuid(),
                GroceryItemId = item.Id,
                OldPrice = item.Price,
                NewPrice = updateDto.Price,
                StoreId = updateDto.StoreId
            });
        }
        item.Name = updateDto.Name;
        item.Size = updateDto.Size;
        item.Price = updateDto.Price;

        await _unitOfWork.SaveChangesAsync();
    }
}