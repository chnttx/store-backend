namespace WebApplication2.Models;

public class PaymentDetail
{
    public Guid PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public string PaymentMethod { get; set; }
    public float Amount { get; set; }
    public DateTime TimeCreated { get; set; }

    public Order Order { get; set; } = null!;
}
