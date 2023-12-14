using WebApplication2.Models;

namespace WebApplication2.Services.Interface;

public interface IOrderService
{
    void CreateOrder();
    void GetOrderById(int orderId);
    void GetAllOrders();
}