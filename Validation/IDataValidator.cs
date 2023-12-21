namespace WebApplication2;

public interface IDataValidator
{
    bool CheckItemIdInDatabase(Guid itemId);
    bool CheckCustomerIdInDatabase(Guid customerId);
    bool CheckKeywordInItemName(string keyword);
    bool CheckOrderInDatabase(Guid orderId);
}