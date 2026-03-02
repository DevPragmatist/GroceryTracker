using Microsoft.AspNetCore.Mvc;

namespace GroceryTracker.Controllers;
[ApiController]
public class GroceryItemController : ControllerBase
{
    public IActionResult AddItem()
    {
        return Ok();
    }
}
