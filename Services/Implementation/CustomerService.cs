using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Request;

namespace WebApplication2.Services.Implementation;

public class CustomerService : ICustomerService
{
    private readonly DataContext _context;

    public CustomerService(DataContext context)
    {
        _context = context;
    }

    public Customer CreateCustomer(CustomerRequest customerRequest)
    {
        throw new NotImplementedException();
    }

    public Customer GetCustomerById(Guid queryCustomerId)
    {
        return _context.Customers.
            Single(c => c.CustomerID == queryCustomerId);
    }

    public ICollection<Customer> GetAllCustomers()
    {
        return _context.Customers.ToList();
    }

    public async Task UpdateVipCustomers()
    {
        Console.WriteLine("Finding and updating VIP customers");
        var vipCustomers = await ( 
            from o in _context.Orders
            
            join oi in _context.OrderItems on o.OrderId equals oi.OrderId
            group oi by o.CustomerId into g
            where g.Select(oi => oi.ItemId).Distinct().Count() > 10 || 
                  g.Sum(oi => oi.ItemPrice * oi.Quantity) > 1e9
            select g.Key).ToListAsync();

        foreach (var customer in vipCustomers.Select(customerId => _context.Customers.Find(customerId)).OfType<Customer>())
        {
            customer.IsVIP = 1;
        }

        await _context.SaveChangesAsync();
    }
}