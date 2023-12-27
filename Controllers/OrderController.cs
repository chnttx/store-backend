using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Request;
using WebApplication2.Response;
using WebApplication2.Services.Interface;
namespace WebApplication2.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/{v:apiVersion}/orders")]
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
    public async Task<IActionResult> GetAllOrders([FromHeader] string secret_key)
    {
        try
        {
            var allOrderResponses = await _orderService.GetAllOrders();
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
    public async Task<IActionResult> GetOrderById([FromHeader] Guid queryOrderId)
    {
        if (!_validator.CheckOrderInDatabase(queryOrderId))
            return NotFound($"Order with '{queryOrderId}' not in database");
        try
        {
            var orderQueryResult = await _orderService.GetOrderById(queryOrderId);
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
    public async Task<IActionResult> GetOrdersByKeyword([FromHeader] string secret_key, string queryKeyword)
    {
        if (!_validator.CheckKeywordInItemName(queryKeyword))
            return NotFound($"No item name containing '{queryKeyword}'");
        try
        {
            var orderKeywordResult = await _orderService.GetOrderByKeyword(queryKeyword);
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
    public async Task<IActionResult> CreateOrder([FromQuery] OrderRequest orderRequest)
    {
        if (!_validator.CheckCustomerIdInDatabase(orderRequest.CustomerId))
            return NotFound($"Customer with id '{orderRequest.CustomerId}' not in database"); 
        try
        {
            var newOrder = await Task.FromResult(_orderService.CreateOrder(orderRequest));
            return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.OrderId }, newOrder);
        }
        catch (Exception e)
        {
            _logger.LogError(e.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occured");
        }
    }

    [HttpDelete("{orderId}/{itemIdToRemove}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveItemFromOrder(Guid itemIdToRemove, Guid orderId)
    {
        if (!_validator.CheckItemIdInDatabase(itemIdToRemove))
            return NotFound($"Item '{itemIdToRemove}' not in database");
        if (!_validator.CheckOrderInDatabase(orderId))
            return NotFound($"Order '{orderId}' not in database");
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

    // [HttpDelete("{orderId}")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task<IActionResult> RemoveOrder(Guid orderId)
    // {
    //     if (!_validator.CheckOrderInDatabase(orderId))
    //         return NotFound($"Order '{orderId}' not in database");
    //     try
    //     {
    //         var orderToRemove = await Task.FromResult(_orderService);
    //     }
    // }
}

