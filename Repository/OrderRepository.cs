using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Repository.RepoInterface;

namespace WebApplication2.Repository;

public class OrderRepository :IOrderRepository
{
    private readonly DataContext _context;

    public OrderRepository(DataContext context)
    {
        _context = context;
    }

    public List<Order> GetOrders()
    {
        return _context.Orders.ToList();
    }

    public Order GetOrderById(int orderId)
    {
        throw new NotImplementedException();
    }

    public Guid CreateOrder()
    {
        throw new NotImplementedException();
    }
    public void DeleteOrderItem()
    {
        throw new NotImplementedException();
    }
}