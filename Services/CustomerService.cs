using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Request;
using WebApplication2.Services.Interface;

namespace WebApplication2.Services;

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
}