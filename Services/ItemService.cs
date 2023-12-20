using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Services.Interface;

namespace WebApplication2.Services;

public class ItemService(DataContext context) : IItemService
{
     public ICollection<Item> GetAllItems()
     {
          ICollection<Item> allItems = context.Items.ToList();

          return allItems;
     }

     public Item GetItemById(Guid itemId)
     {
          ICollection<Item> items = GetAllItems();
          Item item = items.SingleOrDefault(i => i.ItemId == itemId);
          return item;
     }

     public int AddItem(String itemName, float itemPrice, int itemStock, string itemType)
     {
          throw new NotImplementedException();
     }
}