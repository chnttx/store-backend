using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Services.Interface;

namespace WebApplication2.Controllers;

public class OrderController : Controller
{
    private IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    [Route("orders")]
    [HttpGet]
    public ActionResult GetAllOrders()
    {
        var allOrders = _orderService.GetAllOrders();

        return Ok(allOrders);
    }


}