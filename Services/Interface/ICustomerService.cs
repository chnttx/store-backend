namespace WebApplication2.Services.Interface;

public interface ICustomerService
{
    void CreateCustomer();
    void GetCustomerById(int customerId);
    void GetAllCustomers();
}