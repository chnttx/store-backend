using WebApplication2.Models;

namespace WebApplication2.Request;

public class OrderRequest
{
    public Guid CustomerId { get; set; }
    public string DeliveryMethod { get; set; }
    
    public List<OrderItemRequest> OrderItemRequests { get; set; }
}