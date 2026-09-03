using FastEndpoints;
using GroceryTracker.Services.Products.Models;

namespace GroceryTracker.Services.Products;

public class UpsertProductEndpoint : Endpoint<UpsertProductRequestDto>
{
    private readonly IAddItemService _addItemService;
    private readonly IUpdateItemService _updateItemService;
    public UpsertProductEndpoint(IAddItemService addItemService, IUpdateItemService updateItemService)
    {
        _addItemService = addItemService;
        _updateItemService = updateItemService;
    }
    public override void Configure()
    {
        Post("/product");
    }
    public override async Task HandleAsync(UpsertProductRequestDto req, CancellationToken ct)
    {
        if(req.ProductId is null)
        {
            var addDto = new ProductAddDto
            {
                Name = req.Name,
                StoreId = req.StoreId,
                StoreName = req.StoreName,
                Location = req.Location,
                Size = req.Size,
                SizeUnit = req.SizeUnit,
                Price = req.Price,
                PurchaseDate = req.PurchaseDate,
                CategoryId = req.CategoryId,
                CategoryName = req.CategoryName
            };
            await _addItemService.AddNewItem(addDto);
        }
        else
        {
            var updateDto = new ProductUpdateDto
            {
                Id = req.ProductId.Value,
                Name = req.Name,
                StoreId = req.StoreId,
                StoreName = req.StoreName,
                Location = req.Location,
                Size = req.Size,
                SizeUnit = req.SizeUnit,
                Price = req.Price,
                PurchaseDate = req.PurchaseDate
            };
            await _updateItemService.UpdateItem(updateDto);
        }
    }
}