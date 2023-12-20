using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using WebApplication2.Models;
using WebApplication2.Request;
using WebApplication2.Response;
using WebApplication2.Services.Interface;
namespace WebApplication2.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController: Controller
{
    private IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [ProducesResponseType<List<OrderResponse>>(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllOrders()
    {
        // if (!Request.Headers.ContainsKey("secret_key") || Request.Headers["secret_key"].ToString() != "123456")
        //     return StatusCode(403);
        List<OrderResponse> allOrderResponses = await Task.FromResult(_orderService.GetAllOrders());
        return Ok(allOrderResponses);
    }

    [HttpGet("{queryOrderId}")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetOrderById([FromQuery] Guid queryOrderId)
    {
        OrderResponse orderQueryResponse = _orderService.GetOrderById(queryOrderId);
        if (orderQueryResponse == null) return NotFound();
        return Ok(orderQueryResponse);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult CreateOrder([FromQuery] OrderRequest orderRequest)
    {
        Order newOrder = _orderService.CreateOrder(orderRequest);
        return Created();
    }
}