using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models;
[Table("Item")]
public class Item
{
    public Guid ItemId { get; set; }
    [Required]
    public Guid ShopId { get; set; }
    
    [Required]
    [StringLength(255, ErrorMessage = "Name too long")]
    public string ItemName { get; set; }
    
    [Required]
    public string ItemCategory { get; set; }
    
    [Required]
    public float PriceItem { get; set; }
    [Required]
    public int ItemStock { get; set; }
    public Shop Shop { get; set; } = null!;
}