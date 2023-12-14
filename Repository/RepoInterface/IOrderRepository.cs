using WebApplication2.Models;

namespace WebApplication2.Repository.RepoInterface;

public interface IOrderRepository
{
    List<Order> GetOrders();
    Order GetOrderById(int orderId);
    Guid CreateOrder();
    void DeleteOrderItem();
}