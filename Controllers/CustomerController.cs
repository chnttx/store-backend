using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services.Interface;

namespace WebApplication2.Controllers;
[ApiController]
[Route("api/customers")]
public class CustomerController : Controller
{
    private ICustomerService _customerService;
    private IDataValidator _validator;
    private ILogger<CustomerController> _logger;
    public CustomerController(ICustomerService customerService, IDataValidator validator, ILogger<CustomerController> logger)
    {
        _customerService = customerService;
        _validator = validator;
        _logger = logger;
    }
    
    [HttpGet]
    [SecretKey]
    [ProducesResponseType<List<Customer>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetAllCustomers([FromHeader] string secret_key)
    {
        try
        {
            var allCustomers = _customerService.GetAllCustomers();
            return Ok(allCustomers);
        }
        catch (Exception e)
        {
            _logger.LogError(e.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{queryCustomerId}")]
    [SecretKey]
    [ProducesResponseType<Customer>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetCustomerById(string secret_key, Guid queryCustomerId)
    {
        if (!_validator.CheckCustomerIdInDatabase(queryCustomerId))
            throw new Exception($"Customer with ID {queryCustomerId} doesn't exist");
        try
        {
            var customerQueryResult = _customerService.GetCustomerById(queryCustomerId);
            return Ok(customerQueryResult);
        }
        catch (Exception e)
        {
            _logger.LogError(e.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    // [HttpPatch]
    
}