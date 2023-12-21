using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Request;
using WebApplication2.Response;
using WebApplication2.Services.Interface;

namespace WebApplication2.Services;


public class OrderService : IOrderService
{
    private readonly DataContext _context;
    public OrderService(DataContext context)
    {
        _context = context;
    }
    public void CreateOrder()
    {
        throw new NotImplementedException();
    }

    public List<OrderResponse> GetAllOrders()
     {
         var allOrders =
             from o in _context.Orders
             join oi in _context.OrderItems on o.OrderId equals oi.OrderId
             join i in _context.Items on oi.ItemId equals i.ItemId
             join s in _context.Shops on i.ShopId equals s.ShopId
             select new 
             {
                 orderId = o.OrderId,
                 shopName = s.ShopName,
                 itemName = i.ItemName,
                 itemPrice = oi.ItemPrice,
                 quantity = oi.Quantity,
                 dueDate = o.DueDate, 
                 deliveryStatus = o.DeliveryStatus
             };
         // {orderId: [shopName]}
         Dictionary<Guid, List<string>> groupShopsByOrderId = new Dictionary<Guid, List<string>>();
         
         // {orderId: [itemName]}
         Dictionary<Guid, List<string>> groupItemsByOrderId = new Dictionary<Guid, List<string>>();
         foreach (var grouping in allOrders)
         {
             if (groupItemsByOrderId.ContainsKey(grouping.orderId)) 
                 groupItemsByOrderId[grouping.orderId].Add(grouping.itemName);
             else groupItemsByOrderId.Add(grouping.orderId, [grouping.itemName]);
             
             if (groupShopsByOrderId.ContainsKey(grouping.orderId))
                 groupShopsByOrderId[grouping.orderId].Add(grouping.shopName);
             else groupShopsByOrderId.Add(grouping.orderId, [grouping.shopName]);
         }

         var groupByOrderId = allOrders.
             AsEnumerable().GroupBy(o => o.orderId)
             .Select(group => new
             {
                 orderId = group.Key,
                 itemCount = group.Count(),
                 totalCost = group.Sum(oi => (oi.itemPrice * oi.quantity)),
                 group.First().dueDate,
                 group.First().deliveryStatus
             });

         List<OrderResponse> allOrderResponses = new List<OrderResponse>();
         foreach (var g in groupByOrderId)
         {
             List<string> itemsByOrderId = groupItemsByOrderId[g.orderId];
             List<string> shopsByOrderId = groupShopsByOrderId[g.orderId];
             OrderResponse newOrderResponse = new OrderResponse
             {
                 OrderId = g.orderId,
                 DeliveryStatus = g.deliveryStatus,
                 DueDate = g.dueDate,
                 TotalCost = g.totalCost,
                 ItemCount = g.itemCount,
                 allShops = shopsByOrderId,
                 allItems = itemsByOrderId
             };
             allOrderResponses.Add(newOrderResponse);
         }
         return allOrderResponses;
     }

    public OrderResponse GetOrderById(Guid queryOrderId)
    {
        var orderQuery =
            from o in _context.Orders
            join oi in _context.OrderItems on o.OrderId equals oi.OrderId
            join i in _context.Items on oi.ItemId equals i.ItemId
            join s in _context.Shops on i.ShopId equals s.ShopId
            where o.OrderId == queryOrderId
            select new 
            {
                orderId = o.OrderId,
                shopName = s.ShopName,
                itemName = i.ItemName,
                itemPrice = oi.ItemPrice,
                quantity = oi.Quantity,
                dueDate = o.DueDate, 
                deliveryStatus = o.DeliveryStatus
            };
        if (!orderQuery.Any()) return null;
        
        // else 
        List<string> allShopsInOrder = new List<string>();
        List<string> allItemsInOrder = new List<string>();
        var totalCost = orderQuery.Sum(oq => (oq.itemPrice * oq.quantity));
        int itemCount = orderQuery.Count();
        foreach (var grouping in orderQuery)
        {
            allShopsInOrder.Add(grouping.shopName);
            allItemsInOrder.Add(grouping.itemName);
        }

        OrderResponse orderResponseQuery = new OrderResponse
        {
            OrderId = queryOrderId,
            TotalCost = totalCost,
            ItemCount = itemCount,
            DueDate = orderQuery.First().dueDate,
            DeliveryStatus = orderQuery.First().deliveryStatus,
            allShops = allShopsInOrder,
            allItems = allItemsInOrder
        };
        return orderResponseQuery;
    }

    public Order CreateOrder(OrderRequest newOrderRequest)
    {
        // if (!newOrderRequest.OrderItemRequests.Any()) throw new Exception("No items");
        

        Order newOrder = new Order()
        {
            OrderId = Guid.NewGuid(),
            CustomerId = newOrderRequest.CustomerId,
            DeliveryMethod = newOrderRequest.DeliveryMethod,
            DeliveryStatus = "Processing",
            TimeCreated = DateTime.Now,
            DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            Customer = _context.Customers.Single(c => c.CustomerID == newOrderRequest.CustomerId) ?? null, 
            Payments = new List<PaymentDetail>(),
        };
        _context.Orders.Add(newOrder); 
        foreach (OrderItemRequest orderItemRequest in newOrderRequest.OrderItemRequests)
        {
            Item currentItem = _context.Items.Single(i => i.ItemId == orderItemRequest.itemId);
            if (currentItem.ItemStock < orderItemRequest.quantity)
                throw new Exception("Not enough items in stock");
            currentItem.ItemStock -= orderItemRequest.quantity;
            OrderItem newOrderItem = new OrderItem()
            {
                OrderId = newOrder.OrderId,
                ItemId = currentItem.ItemId,
                ItemPrice = orderItemRequest.itemPrice,
                Quantity = orderItemRequest.quantity
            };
            _context.OrderItems.Add(newOrderItem);
        }
        _context.SaveChanges();
        return newOrder;
    }

    public OrderItem RemoveItemFromOrder(Guid idOfItemToRemove, Guid orderId)
    {
        IQueryable<OrderItem> allOrderItemById = (
            from oi in _context.OrderItems
            where oi.OrderId == orderId
            select oi);
        OrderItem orderItemToRemove = null;
        foreach (OrderItem oi in allOrderItemById)
        {
            if (oi.ItemId != idOfItemToRemove) continue;
            orderItemToRemove = oi;
            break;
        }

        if (orderItemToRemove == null) throw new Exception("Item not in order");
        Item itemToRemove = _context.Items.First(i => i.ItemId == idOfItemToRemove);
        itemToRemove.ItemStock += orderItemToRemove.Quantity;
        _context.OrderItems.Remove(orderItemToRemove);
        _context.SaveChanges();
        return orderItemToRemove;
    }

    public List<Guid> GetOrderByKeyword(string queryKeyword)
    {
        var orderQueryByKeyword = (
            from o in _context.Orders
            join oi in _context.OrderItems on o.OrderId equals oi.OrderId
            join i in _context.Items on oi.ItemId equals i.ItemId
            where i.ItemName.ToLower().Contains(queryKeyword)
            select o.OrderId).ToList();

        return orderQueryByKeyword;
    }
}