using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models;

public class OrderItem
{
    public Guid OrderId { get; set; }
    public Guid ItemId { get; set;  }
    
    [Required]
    public float ItemPrice { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public float TotalPrice { get; set; } 

    public Order Order { get; set; } = null!;
    public Item Item { get; set; } = null!;

}