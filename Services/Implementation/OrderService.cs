using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Request;
using WebApplication2.Response;
using WebApplication2.Services.Interface;

namespace WebApplication2.Services.Implementation;


public class OrderService : IOrderService
{
    private readonly DataContext _context;
    public OrderService(DataContext context)
    {
        _context = context;
    }
    public async Task<List<OrderResponse>> GetAllOrders()
     {
         var allOrders = await _context.Orders
             .Join(_context.OrderItems, o => o.OrderId, oi => oi.OrderId, (o, oi) => new { Order = o, OrderItem = oi })
             .Join(_context.Items, oi => oi.OrderItem.ItemId, i => i.ItemId, (oi, i) => new { oi.Order, oi.OrderItem, Item = i })
             .Join(_context.Shops, i => i.Item.ShopId, s => s.ShopId, (i, s) => new
             {
                 orderId = i.Order.OrderId,
                 shopName = s.ShopName,
                 itemName = i.Item.ItemName,
                 itemPrice = i.OrderItem.ItemPrice,
                 quantity = i.OrderItem.Quantity,
                 dueDate = i.Order.DueDate,
                 deliveryStatus = i.Order.DeliveryStatus
             })
             .ToListAsync();

         
         // {orderId: [shopName]}
         Dictionary<Guid, List<string>> groupShopsByOrderId = new Dictionary<Guid, List<string>>();
         
         // {orderId: [itemName]}
         Dictionary<Guid, List<OrderItemResponse>> groupOrderItemsByOrderId = new Dictionary<Guid, List<OrderItemResponse>>();
         foreach (var grouping in allOrders)
         {
             OrderItemResponse orderItemResponse = new OrderItemResponse()
             {
                 itemName = grouping.itemName,
                 itemQuantity = grouping.quantity,
                 itemPrice = grouping.itemPrice,
                 totalPrice = grouping.itemPrice * grouping.quantity
             };
             
             if (groupOrderItemsByOrderId.ContainsKey(grouping.orderId))
                 groupOrderItemsByOrderId[grouping.orderId].Add(orderItemResponse);
             else groupOrderItemsByOrderId.Add(grouping.orderId, [orderItemResponse]);
             
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
             List<string> shopsByOrderId = groupShopsByOrderId[g.orderId];
             List<OrderItemResponse> orderItemsByOrderId = groupOrderItemsByOrderId[g.orderId];
             OrderResponse newOrderResponse = new OrderResponse
             {
                 OrderId = g.orderId,
                 DeliveryStatus = g.deliveryStatus,
                 DueDate = g.dueDate,
                 TotalCost = g.totalCost,
                 ItemCount = g.itemCount,
                 allShops = shopsByOrderId,
                 allItems = orderItemsByOrderId
             };
             allOrderResponses.Add(newOrderResponse);
         }
         return allOrderResponses;
     }

    public async Task<OrderResponse> GetOrderById(Guid queryOrderId)
    {
        var orderQuery = await _context.Orders
            .Join(_context.OrderItems, o => o.OrderId, oi => oi.OrderId, (o, oi) => new { Order = o, OrderItem = oi })
            .Join(_context.Items, oi => oi.OrderItem.ItemId, i => i.ItemId, (oi, i) => new { oi.Order, oi.OrderItem, Item = i })
            .Join(_context.Shops, i => i.Item.ShopId, s => s.ShopId, (i, s) => new
            {
                orderId = i.Order.OrderId,
                shopName = s.ShopName,
                itemName = i.Item.ItemName,
                itemPrice = i.OrderItem.ItemPrice,
                quantity = i.OrderItem.Quantity,
                dueDate = i.Order.DueDate,
                deliveryStatus = i.Order.DeliveryStatus
            })
            .Where(x => x.orderId == queryOrderId)
            .ToListAsync();

        
        if (orderQuery.Count == 0) return null!;
        
        // else 
        List<string> allShopsInOrder = new List<string>();
        List<OrderItemResponse> allItemsInOrder = new List<OrderItemResponse>();
        var totalCost = orderQuery.Sum(oq => (oq.itemPrice * oq.quantity));
        int itemCount = orderQuery.Count();
        foreach (var grouping in orderQuery)
        {
            OrderItemResponse orderItemResponse = new OrderItemResponse()
            {
                itemName = grouping.itemName,
                itemPrice = grouping.itemPrice,
                itemQuantity = grouping.quantity,
                totalPrice = grouping.itemPrice * grouping.quantity
            };
            allShopsInOrder.Add(grouping.shopName);
            allItemsInOrder.Add(orderItemResponse);
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
        Order newOrder = new Order()
        {
            OrderId = Guid.NewGuid(),
            CustomerId = newOrderRequest.CustomerId,
            DeliveryMethod = newOrderRequest.DeliveryMethod,
            DeliveryStatus = "Processing",
            TimeCreated = DateTime.Now,
            DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            Customer = (_context.Customers.Single(c => c.CustomerID == newOrderRequest.CustomerId) ?? null)!, 
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
                ItemPrice = currentItem.ItemPrice,
                Quantity = orderItemRequest.quantity
            };
            _context.OrderItems.Add(newOrderItem);
        }
        _context.SaveChanges();
        return newOrder;
    }

    public async Task<OrderItem> RemoveItemFromOrder(Guid idOfItemToRemove, Guid orderId)
    {
        var allOrderItemById = await (
            from oi in _context.OrderItems
            where oi.OrderId == orderId
            select oi).ToListAsync();
        
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
        await _context.SaveChangesAsync();
        return orderItemToRemove;
    }

    public Task<OrderResponse> DeleteOrder(Guid orderId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<OrderResponse>> GetOrderByKeyword(string queryKeyword)
    {
        var orderQueryByKeyword = await (
            from o in _context.Orders
            join oi in _context.OrderItems on o.OrderId equals oi.OrderId
            join i in _context.Items on oi.ItemId equals i.ItemId
            where i.ItemName.ToLower().Contains(queryKeyword)
            select new { Order = o, OrderItem = oi, Item = i }).ToListAsync();

        var groupedOrders = orderQueryByKeyword.GroupBy(x => x.Order.OrderId);

        List<OrderResponse> resultOrderQueryByKeyword = new List<OrderResponse>();
        foreach (var group in groupedOrders)
        {
            float totalCost = 0;
            List<string> allShops = new List<string>();
            List<OrderItemResponse> allItemsInResponse = new List<OrderItemResponse>();

            foreach (var order in group)
            {
                totalCost += order.OrderItem.Quantity * order.OrderItem.ItemPrice;
                OrderItemResponse orderItemResponse = new OrderItemResponse()
                {
                    itemName = order.Item.ItemName,
                    itemQuantity = order.OrderItem.Quantity,
                    itemPrice = order.OrderItem.ItemPrice,
                    totalPrice = order.OrderItem.ItemPrice * order.OrderItem.Quantity,
                };
                allItemsInResponse.Add(orderItemResponse);
                allShops.Add(order.Item.Shop.ShopName);
            }

            OrderResponse resultOrderResponse = new OrderResponse()
            {
                OrderId = group.Key,
                TotalCost = totalCost,
                ItemCount = group.Count(),
                DueDate = group.First().Order.DueDate,
                DeliveryStatus = group.First().Order.DeliveryStatus,
                allShops = allShops,
                allItems = allItemsInResponse
            };

            resultOrderQueryByKeyword.Add(resultOrderResponse);
        }
        return resultOrderQueryByKeyword;
    }
    
}