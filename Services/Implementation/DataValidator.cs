using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2;

public class DataValidator : IDataValidator
{
    private readonly DataContext _context;

    public DataValidator(DataContext context)
    {
        _context = context;
    }
    
    public bool CheckItemIdInDatabase(Guid itemId)
    {
        return Enumerable.Any(_context.Items, i => i.ItemId == itemId);
    }

    public bool CheckCustomerIdInDatabase(Guid customerId)
    {
        return Enumerable.Any(_context.Customers, c => c.CustomerID == customerId);
    }

    public bool CheckKeywordInItemName(string keyword)
    {
        return Enumerable.Any(_context.Items, i => i.ItemName.ToLower().Contains(keyword));
    }

    public bool CheckOrderInDatabase(Guid orderId)
    {
        return Enumerable.Any(_context.Orders, o => o.OrderId == orderId);
    }

// orderId    public bool CheckValidDeliveryMethod(string deliveryMethod)
//     {
//         return deliveryMethod.ToLower().Equals("express") || deliveryMethod.ToLower().Equals("standard");
//     }
    
}