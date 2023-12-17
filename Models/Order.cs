using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Enums;

namespace WebApplication2.Models;

[Table("Orders")]
public class Order
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    
    [Required]
    public string DeliveryMethod { get; set; }
    
    [Required]
    public OrderStatusEnum DeliveryStatus { get; set; }
    
    [Required]
    public DateTime TimeCreated { get; set; }
    
    [Required]
    public DateOnly DueDate { get; set; }

    public Customer Customer { get; set; } = null!;

    public ICollection<PaymentDetail> Payments = null!;
}