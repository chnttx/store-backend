using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers;

public class CustomerController : Controller
{
    private CustomerService _customerService;

    public CustomerController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [Route("customers")]
    public IActionResult GetAllCustomers()
    {
        var allCustomers = _customerService.GetAllCustomers();

        return Ok(allCustomers);
    }
    
}