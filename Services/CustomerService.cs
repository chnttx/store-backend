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

    public void CreateCustomer(CustomerRequest customerRequest)
    {
        throw new NotImplementedException();
    }

    public void GetCustomerById(int customerId)
    {
        throw new NotImplementedException();
    }

    public ICollection<Customer> GetAllCustomers()
    {
        throw new NotImplementedException();
    }

    // public Order MapOrderRequestToOrder(OrderRequest orderRequest)
    // {
    //     Order newOrderResult = new Order();
    //     newOrderResult.OrderId = Guid.NewGuid();
    //     newOrderResult.CustomerId = orderRequest.CustomerId;
    //     newOrderResult.TimeCreated = DateTime.Now;
    //     newOrderResult.DeliveryStatus = "Processing";
    //     newOrderResult.DeliveryMethod = orderRequest.DeliveryMethod;
    //     DateTime newDueDateTime = newOrderResult.TimeCreated.AddDays(7);
    //     DateOnly newDueDate = newDueDateTime.;
    //
    //
    // }
}