using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
    //     var customers =
    // }
}