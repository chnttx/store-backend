using WebApplication2.Models;

namespace WebApplication2.Services.Interface;

public interface IShopService
{
    void AddItem(Item item);
    void RemoveItem(Guid itemId);
}