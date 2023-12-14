using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Services.Interface;

namespace WebApplication2.Services;


public class OrderService : IOrderService
{
    // private readonly DataContext _context;
    //
    // public OrderService(DataContext context)
    // {
    //     _context = context;
    // }
    
    public void CreateOrder()
    {
        throw new NotImplementedException();
    }

    public void GetOrderById(int orderId)
    {
        throw new NotImplementedException();
    }

    public void GetAllOrders()
    {
        throw new NotImplementedException();
    }

    public void UpdateAddress()
    {
        throw new NotImplementedException();
    }
}