using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models;
[Table("Shop")]
public class Shop
{
    public Guid ShopId { get; set; }
    
    [Required]
    [StringLength(255, ErrorMessage = "ShopName cannot exceed 255 characters")]
    public string ShopName { get; set; }
    
    [Required]
    [StringLength(255, ErrorMessage = "ShopAddress cannot exceed 255 characters")]
    public string ShopAddress { get; set; }
    
    [Required]
    [StringLength(255, ErrorMessage = "ShopEmail must be valid")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email")]
    public string ShopEmail { get; set; }
    
    [Required]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Phone Number")]
    public string ShopPhoneNo { get; set; }
    public string? ShopDescription { get; set; }

    public ICollection<Item> Items { get; set; } = null!;
}