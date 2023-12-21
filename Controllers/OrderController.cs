using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Request;
using WebApplication2.Response;
using WebApplication2.Services.Interface;
namespace WebApplication2.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController: Controller
{
    private readonly ILogger<OrderController> _logger;
    private IOrderService _orderService;
    private IDataValidator _validator;

    public OrderController(IOrderService orderService, IDataValidator validator, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _validator = validator;
        _logger = logger;
    }

    [HttpGet]
    [SecretKey]
    [ProducesResponseType<List<OrderResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetAllOrders([FromHeader] string secret_key)
    {
        try
        {
            var allOrderResponses = _orderService.GetAllOrders();
            return Ok(allOrderResponses);
        }
        catch (Exception e)
        {
            _logger.LogError(e.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("id/{queryOrderId}")]
    [SecretKey]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetOrderById([FromHeader] string secret_key, Guid queryOrderId)
    {
        if (!_validator.CheckOrderInDatabase(queryOrderId))
            throw new Exception($"No order with id {queryOrderId}");
        try
        {
            var orderQueryResult = _orderService.GetOrderById(queryOrderId);
            return Ok(orderQueryResult);
        }
        catch (Exception e)
        {
            _logger.LogError(e.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("keyword/{queryKeyword}")]
    [SecretKey]
    [ProducesResponseType<List<OrderResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetOrdersByKeyword([FromHeader] string secret_key, string queryKeyword)
    {
        if (!_validator.CheckKeywordInItemName(queryKeyword))
            throw new Exception($"No item containing {queryKeyword}");
        try
        {
            var orderKeywordResult = _orderService.GetOrderByKeyword(queryKeyword);
            return Ok(orderKeywordResult);
        }
        catch (Exception e)
        {
            _logger.LogError(e.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred");
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateOrder([FromQuery] OrderRequest orderRequest)
    {
        try
        {
            var newOrder = _orderService.CreateOrder(orderRequest);
            return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.OrderId }, newOrder);
        }
        catch (Exception e)
        {
            _logger.LogError(e.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occured");
        }
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveItemFromOrder(Guid itemIdToRemove, Guid orderId)
    {
        if (!_validator.CheckItemIdInDatabase(itemIdToRemove))
            throw new Exception($"Item {itemIdToRemove} not in database");
        if (!_validator.CheckOrderInDatabase(orderId))
            throw new Exception($"Order {orderId} not in database");

        try
        {
            var orderItemToRemove = await Task.FromResult(_orderService.RemoveItemFromOrder(itemIdToRemove, orderId));
            return Ok(orderItemToRemove);
        }
        catch (Exception e)
        {
            _logger.LogError(e.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occured");
        }
    }
}

