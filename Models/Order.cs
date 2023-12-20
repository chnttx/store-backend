using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models;

[Table("OrderTable")]
public class Order
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    
    [Required]
    public string DeliveryMethod { get; set; }
    
    [Required]
    public string DeliveryStatus { get; set; }
    
    [Required]
    public DateTime TimeCreated { get; set; }
    
    [Required]
    public DateOnly DueDate { get; set; }

    [Required]
    public Customer Customer { get; set; } = null!;

    public ICollection<PaymentDetail> Payments = null!;
}