using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Services;

public class HourlyBackgroundService : BackgroundService
{
    private DataContext _context;
    private ILogger<HourlyBackgroundService> _logger;

    public HourlyBackgroundService(DataContext context, ILogger<HourlyBackgroundService> logger)
    {
        _logger = logger;
        _context = context;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var currentTime = DateTime.UtcNow;
            if (currentTime is { Minute: 0, Second: 0 })
            {
                _logger.LogInformation("Updating VIP customers");
                UpdateVipCustomers();
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private void UpdateVipCustomers()
    {
        var vipCustomers = ( 
            from o in _context.Orders
            join oi in _context.OrderItems on o.OrderId equals oi.OrderId
            group oi by o.CustomerId into g
                where g.Select(oi => oi.ItemId).Distinct().Count() > 10 || 
                      g.Sum(oi => oi.ItemPrice * oi.Quantity) > 1e6
                      select g.Key).ToList();
        foreach (var customer in vipCustomers.Select(customerId => _context.Customers.Find(customerId)).OfType<Customer>())
        {
            customer.IsVIP = 1;
        }

        _context.SaveChanges();
    }
}