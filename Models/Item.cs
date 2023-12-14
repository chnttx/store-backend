using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models;

public class Item
{
    public int ItemId { get; set; }
    public int ShopId { get; set; }
    
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