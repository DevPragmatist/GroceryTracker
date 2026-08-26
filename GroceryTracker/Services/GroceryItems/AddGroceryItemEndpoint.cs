using FastEndpoints;
using GroceryTracker.Services.GroceryItems.Models;

namespace GroceryTracker.Services.GroceryItems;

public class AddGroceryItemEndpoint : Endpoint<GroceryItemAddDto>
{
    private readonly IAddItemService _addItem;
    public AddGroceryItemEndpoint(IAddItemService addItemservice)
    {
        _addItem = addItemservice;
    }
    public override void Configure()
    {
        Post("/groceryitems");
        AllowAnonymous();
    }
    public override async Task HandleAsync(GroceryItemAddDto req, CancellationToken ct)
    {
        await _addItem.AddNewItem(req);            
    }
}