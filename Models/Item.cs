using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models;

public class Item
{
    public Guid ItemId { get; set; }
    public Guid ShopId { get; set; }
    
    [Required]
    [StringLength(255, ErrorMessage = "Name too long")]
    public string ItemName { get; set; }
    
    [Required]
    public string ItemCategory { get; set; }
    
    [Required]
    public float ItemPrice { get; set; }
    [Required]
    public int ItemStock { get; set; }

    public Shop Shop { get; set; } = null!;
}