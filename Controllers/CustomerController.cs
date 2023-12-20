using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;
using WebApplication2.Services.Interface;

namespace WebApplication2.Controllers;

public class CustomerController(ICustomerService customerService) : Controller
{
    [Route("customers")]
    [HttpGet]
    public ActionResult GetAllCustomers()
    {
        ICollection<Customer> allCustomers = customerService.GetAllCustomers();

        return Ok(allCustomers);
    }
    
}