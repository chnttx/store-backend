using WebApplication2.Models;
using WebApplication2.Request;
using WebApplication2.Response;

namespace WebApplication2.Services.Interface;

public interface IOrderService
{
    Order CreateOrder(OrderRequest newOrderRequest);
    Task<OrderResponse> GetOrderById(Guid queryOrderId);
    Task<List<OrderResponse>> GetAllOrders();
    Task<List<OrderResponse>> GetOrderByKeyword(string queryKeyword);
    Task<OrderItem> RemoveItemFromOrder(Guid idOfItemToRemove, Guid orderId);
    Task<OrderResponse> DeleteOrder(Guid orderId);
}