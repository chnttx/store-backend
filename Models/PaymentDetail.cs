namespace WebApplication2.Models;

public class PaymentDetail
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public string PaymentMethod { get; set; }
    public float Amount { get; set; }
    public DateTime TimeCreated { get; set; }

    public Order Order { get; set; } = null!;
}
