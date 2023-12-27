using WebApplication2.Models;
using WebApplication2.Request;

namespace WebApplication2.Services.Interface;

public interface ICustomerService
{
    Customer CreateCustomer(CustomerRequest customerRequest);
    Customer GetCustomerById(Guid queryCustomerId);
    ICollection<Customer> GetAllCustomers();
}