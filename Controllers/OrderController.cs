using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WebApplication2.Request;
using WebApplication2.Response;
using WebApplication2.Services;
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
    public async Task<IActionResult> GetAllOrders([FromHeader] string secret_key, [FromQuery] PaginationFilter filter)
    {
        try
        {
            var allOrderResponses = await _orderService.GetAllOrders();
            var pagedData = allOrderResponses
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
            return Ok(new PagedResponse<List<OrderResponse>>(pagedData, filter.PageNumber, filter.PageSize));
        }
        catch (Exception e)
        {
            _logger.LogError(e.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("all")]
    public async Task<IActionResult> getOrders()
    {
        var allOrders = await _orderService.GetOrders();
        return Ok(allOrders);
    }
    
    [HttpGet("id/{queryOrderId}")]
    [SecretKey]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrderById([FromHeader] string secret_key, Guid queryOrderId)
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrdersByKeyword([FromHeader] string secret_key, string queryKeyword, 
        [FromQuery] PaginationFilter filter)
    {
        if (!_validator.CheckKeywordInItemName(queryKeyword))
            return NotFound($"No item name containing '{queryKeyword}'");
        try
        {
            var orderKeywordResult = await _orderService.GetOrderByKeyword(queryKeyword);
            var pagedData = orderKeywordResult
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
            return Ok(new PagedResponse<List<OrderResponse>>(pagedData, filter.PageNumber, filter.PageSize));
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
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest orderRequest)
    {
        if (!_validator.CheckCustomerIdInDatabase(orderRequest.CustomerId))
            return NotFound($"Customer with id '{orderRequest.CustomerId}' not in database");
        if (!_validator.CheckValidDeliveryMethod(orderRequest.DeliveryMethod))
            return BadRequest($"Invalid delivery method");
        if (orderRequest.OrderItemRequests.Count == 0)
            throw new ArgumentException("Order can't be empty");
        try
        {
            var newOrder = await _orderService.CreateOrder(orderRequest);
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveItemFromOrder(Guid itemIdToRemove, Guid orderId)
    {
        if (!_validator.CheckItemIdInDatabase(itemIdToRemove))
            return NotFound($"Item '{itemIdToRemove}' not in database");
        if (!_validator.CheckOrderInDatabase(orderId))
            return NotFound($"Order '{orderId}' not in database");
        try
        {
            var orderItemToRemove = await _orderService.RemoveItemFromOrder(itemIdToRemove, orderId);
            return Ok(orderItemToRemove);
        }
        catch (Exception e)
        {
            _logger.LogError(e.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occured");
        }
    }
}

