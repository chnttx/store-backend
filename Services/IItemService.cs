using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Services.Interface;

public interface IItemService
{
    ICollection<Item> GetAllItems();

    Item GetItemById(Guid itemId);
    int AddItem(string itemName, float itemPrice, int itemStock, string Category);
    
}