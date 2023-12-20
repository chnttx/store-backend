using WebApplication2.Data;

namespace WebApplication2;

public class DataValidator
{
    private readonly DataContext _context;

    public DataValidator(DataContext context)
    {
        _context = context;
    }
    
    public bool CheckItemIdInDatabase(Guid itemId)
    {
        List<Guid> allItemIdsInDatabase = (
            from i in _context.Items
            select i.ItemId
        ).ToList();

        return allItemIdsInDatabase.Contains(itemId);
    }

    public bool CheckCustomerIdInDatabase(Guid customerId)
    {
        List<Guid> allCustomerIdInDatabase = (
            from c in _context.Customers
            select c.CustomerID).ToList();

        return allCustomerIdInDatabase.Contains(customerId);
    }
    
}