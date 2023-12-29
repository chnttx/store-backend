using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models;
[Table("OrderItem")]
public class OrderItem
{
    public Guid OrderId { get; set; }
    public Guid ItemId { get; set;  }
    
    [Required]
    public float ItemPrice { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public float TotalCost { get; set; } 

    public Order Order { get; set; } = null!;
    public Item Item { get; set; } = null!;

}