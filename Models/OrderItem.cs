namespace WebApplication2.Models;

public class OrderItem
{
    public int OrderId { get; set; }
    public int ItemId { get; set;  }
    public required float ItemPrice { get; set; }
    public required int Quantity { get; set; }
    public float TotalPrice { get; set; } 

    public Order Order { get; set; } = null!;
    public Item Item { get; set; } = null!;

}