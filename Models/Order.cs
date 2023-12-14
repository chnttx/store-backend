using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models;

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    
    [Required]
    public string DeliveryMethod { get; set; }
    
    [Required]
    public string DeliveryStatus { get; set; }
    
    [Required]
    public DateTime TimeCreated { get; set; }
    
    [Required]
    public DateOnly DueDate { get; set; }

    public Customer Customer { get; set; } = null!;

    public ICollection<PaymentDetail> Payments = null!;
}