using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Request;
using WebApplication2.Response;

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

    public async Task<Order> CreateOrder(OrderRequest newOrderRequest)
    {
        Order newOrder = new Order()
        {
            OrderId = Guid.NewGuid(),
            CustomerId = newOrderRequest.CustomerId,
            DeliveryMethod = newOrderRequest.DeliveryMethod,
            DeliveryStatus = "Processing",
            TimeCreated = DateTime.Now,
            DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            Customer = await _context.Customers.SingleAsync(c => c.CustomerID == newOrderRequest.CustomerId), 
            Payments = new List<PaymentDetail>(),
        };
        
        _context.Orders.Add(newOrder); 
        foreach (OrderItemRequest orderItemRequest in newOrderRequest.OrderItemRequests)
        {
            var currentItem = await _context.Items.SingleAsync(i => i.ItemId == orderItemRequest.itemId);
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
        await _context.SaveChangesAsync();
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
        Item itemToRemove = await _context.Items.FirstAsync(i => i.ItemId == idOfItemToRemove);
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
            where i.ItemName.ToLower().Contains(queryKeyword.ToLower())
            select new
            {
                orderId = o.OrderId,
                dueDate = o.DueDate,
                deliveryStatus = o.DeliveryStatus
                
            })
            .DistinctBy(ooi => ooi.orderId)
            .ToListAsync();

        List<OrderResponse> resultOrderQueryByKeyword = new List<OrderResponse>();
        foreach (var orderDetail in orderQueryByKeyword)
        {
            List<OrderItemResponse> allItemsInOrder = new List<OrderItemResponse>();
            List<string> allShops = new List<string>();
            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == orderDetail.orderId).Include(orderItem => orderItem.Item)
                .ThenInclude(item => item.Shop).ToListAsync();
            int itemCount = 0;
            float totalOrderPrice = 0;
            foreach (var orderItem in orderItems)
            {
                OrderItemResponse newOrderItemResponse = new OrderItemResponse()
                {
                    itemName = orderItem.Item.ItemName,
                    itemPrice = orderItem.ItemPrice,
                    itemQuantity = orderItem.Quantity,
                    totalPrice = orderItem.ItemPrice * orderItem.Quantity
                };
                allItemsInOrder.Add(newOrderItemResponse);
                allShops.Add(orderItem.Item.Shop.ShopName);
            }

            OrderResponse newOrderResponse = new OrderResponse()
            {
                OrderId = orderDetail.orderId,
                TotalCost = totalOrderPrice,
                ItemCount = itemCount,
                DueDate = orderDetail.dueDate,
                DeliveryStatus = orderDetail.deliveryStatus,
                allItems = allItemsInOrder,
                allShops = allShops,
            }; resultOrderQueryByKeyword.Add(newOrderResponse);
        }

        return resultOrderQueryByKeyword;
    }
    
}