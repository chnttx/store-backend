using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services.Interface;

namespace WebApplication2.Controllers;

public class ItemController(IItemService itemService) : Controller
{
    [Route("items")]
    [HttpGet]
    public ActionResult GetAllItems()
    {
        ICollection<Item> allOrders = itemService.GetAllItems();
        return Ok(allOrders);
    }
}