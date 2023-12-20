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
    private readonly DataContext context;
    public OrderService(DataContext context1)
    {
        context = context1;
    }
    public void CreateOrder()
    {
        throw new NotImplementedException();
    }

    public OrderResponse GetOrderById(Guid orderId)
    {
        throw new NotImplementedException();
    }

    public List<OrderResponse> GetAllOrders()
     {
         var allOrders =
             from o in context.Orders
             join oi in context.OrderItems on o.OrderId equals oi.OrderId
             join i in context.Items on oi.ItemId equals i.ItemId
             join s in context.Shops on i.ShopId equals s.ShopId
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

    OrderResponse IOrderService.GetOrderById(Guid queryOrderId)
    {
        var orderQuery =
            from o in context.Orders
            join oi in context.OrderItems on o.OrderId equals oi.OrderId
            join i in context.Items on oi.ItemId equals i.ItemId
            join s in context.Shops on i.ShopId equals s.ShopId
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
        float totalCost = orderQuery.Sum(oq => (oq.itemPrice * oq.quantity));
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
        if (!newOrderRequest.OrderItemRequests.Any()) return null;

        Order newOrder = new Order()
        {
            OrderId = Guid.NewGuid(),
            CustomerId = newOrderRequest.CustomerId,
            DeliveryMethod = newOrderRequest.DeliveryMethod,
            DeliveryStatus = "Processing",
            TimeCreated = DateTime.Now,
            DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            Customer = context.Customers.FirstOrDefault(c => c.CustomerID == newOrderRequest.CustomerId) ?? null, 
            Payments = new List<PaymentDetail>(),
        };
        context.Orders.Add(newOrder); 
        foreach (OrderItemRequest orderItemRequest in newOrderRequest.OrderItemRequests)
        {
            Item currentItem = context.Items.Single(i => i.ItemId == orderItemRequest.itemId);
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
            context.OrderItems.Add(newOrderItem);
        }
        context.SaveChanges();
        return newOrder;
    }

    public void RemoveItemFromOrder(Guid idOfItemToRemove, Guid orderId)
    {
        
    }
}