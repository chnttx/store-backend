using System.ComponentModel.DataAnnotations;
using WebApplication2.Models;

namespace WebApplication2.Request;

public class CustomerRequest
{
    [Required]
    public string CustomerName { get; set; }
    [Required]
    [DataType(DataType.PhoneNumber, ErrorMessage = "Must be valid phone number")]
    public string CustomerPhoneNo { get; set; }

    [Required]
    [StringLength(255, ErrorMessage = "Email too long")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email")]
    public string CustomerEmail { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateOnly DateOfBirth { get; set; }
    
    [Required]
    public string CustomerAddress { get; set; }
    
    [StringLength(19, MinimumLength = 19, ErrorMessage = "Invalid card")]
    public string? CardNumber { get; set; }
    
}