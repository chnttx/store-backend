using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models;

public class Customer
{
    public Guid CustomerID { get; set; }
    
    [Required]
    [StringLength(255, ErrorMessage = "Name length too long")]
    public string CustomerName { get; set; }
    
    [Required]
    [StringLength(255, ErrorMessage = "Email too long")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email")]
    public string? CustomerEmail { get; set; }
    
    [Required]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Number")]
    public string CustomerPhoneNo { get; set; }

    public DateOnly DateOfBirth { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CardNumber { get; set; }
    
    [Required]
    public Byte IsVIP { get; set; }

    public ICollection<Order> Orders { get; set; } = null!;
}