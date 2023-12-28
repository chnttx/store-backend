namespace WebApplication2.Response;

public class OrderItemResponse
{
    public string itemName { get; set; }
    public int itemQuantity { get; set; }
    public float totalPrice { get; set; }
    public float itemPrice { get; set; }
}