using WebApplication2.Models;

namespace WebApplication2.Response;

public class OrderResponse 
{
    public Guid OrderId { get; set; }
    public float TotalCost { get; set; }
    public int ItemCount { get; set; }
    public DateOnly DueDate { get; set; }
    public string DeliveryStatus { get; set; }
    public List<string> allShops { get; set; }
    public List<string> allItems { get; set; }
}
