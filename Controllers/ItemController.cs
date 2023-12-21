using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services.Interface;

namespace WebApplication2.Controllers;
[ApiController]
[Route("api/items")]
public class ItemController : Controller
{
    private IItemService _itemService;
    private IDataValidator _validator;
    private ILogger<ItemController> _logger;

    public ItemController(IItemService itemService, IDataValidator validator, ILogger<ItemController> logger)
    {
        _itemService = itemService;
        _validator = validator;
        _logger = logger;
    }
    [HttpGet]
    [SecretKey]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAllItems([FromHeader]string secret_key)
    {
        try
        {
            ICollection<Item> allOrders = _itemService.GetAllItems();
            return Ok(allOrders);
        }
        catch (Exception e)
        {
            _logger.LogError(e.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}