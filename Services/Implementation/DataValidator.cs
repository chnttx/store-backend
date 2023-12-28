using WebApplication2.Data;

namespace WebApplication2.Services.Implementation;

public class DataValidator : IDataValidator
{
    private static readonly string[] ALL_DELIVERY_METHODS = {
        "standard", "express"
    };
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

    public bool CheckValidDeliveryMethod(string deliveryMethod)
    {
        deliveryMethod = deliveryMethod.Trim().ToLower();
        return Array.Exists(ALL_DELIVERY_METHODS, w => w == deliveryMethod);
    }
    
}