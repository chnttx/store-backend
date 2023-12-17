using Microsoft.AspNetCore.Mvc;
using WebApplication2.Services.Interface;

namespace WebApplication2.Controllers;

public class ItemController : Controller
{
    private IItemService _itemService;

    public ItemController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [Route("Items")]
    [HttpGet]
    public IActionResult GetAllItems()
    {
        var allOrders = _itemService.GetAllItems();

        return Ok(allOrders);
    }
}