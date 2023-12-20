using WebApplication2.Models;
using WebApplication2.Request;
using WebApplication2.Response;

namespace WebApplication2.Services.Interface;

public interface IOrderService
{
    Order CreateOrder(OrderRequest newOrderRequest);
    OrderResponse GetOrderById(Guid queryOrderId);
    List<OrderResponse> GetAllOrders();
}