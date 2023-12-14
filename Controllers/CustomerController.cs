using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApplication2.Data;
using WebApplication2.Models;
namespace WebApplication2.Controllers;

public class CustomerController : Controller
{
    private readonly DataContext _context;

    public CustomerController(DataContext context)
    {
        _context = context;
    }

    // public IActionResult Index()
    // {
    //     try
    //     {
    //         // Attempt to query the database to check the connection
    //         var firstItem = _context.Customers.FirstOrDefault();
    //
    //         // If no exception is thrown, the connection is successful
    //         ViewBag.Message = "Connected to the database!";
    //     }
    //     catch (Exception ex)
    //     {
    //         // Handle connection errors
    //         ViewBag.Message = $"Error connecting to the database: {ex.Message}";
    //     }
    //
    //     return Ok();
    // }
}