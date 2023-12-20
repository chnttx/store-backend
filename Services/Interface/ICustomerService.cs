using WebApplication2.Models;
using WebApplication2.Request;

namespace WebApplication2.Services.Interface;

public interface ICustomerService
{
    void CreateCustomer(CustomerRequest customerRequest);
    void GetCustomerById(int customerId);
    ICollection<Customer> GetAllCustomers();
}