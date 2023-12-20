using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models;
[Table("Customer")]
public class Customer
{
    public Guid CustomerID { get; set; }
    
    [Required]
    [StringLength(255, ErrorMessage = "Name length too long")]
    public string CustomerName { get; set; }
    
    [Required]
    [StringLength(255, ErrorMessage = "Email too long")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email")]
    public string CustomerEmail { get; set; }
    
    [Required]
    [DataType(DataType.PhoneNumber)]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Number")]
    public string CustomerPhoneNo { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateOnly DateOfBirth { get; set; }
    
    [Required]
    public string CustomerAddress { get; set; }
    public string? CardNumber { get; set; }
    
    [Required]
    public Byte IsVIP { get; set; }

    public ICollection<Order> Orders { get; set; } = null!;
}