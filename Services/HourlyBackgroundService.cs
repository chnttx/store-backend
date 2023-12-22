using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Services;

public class HourlyBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HourlyBackgroundService> _logger;

    public HourlyBackgroundService(IServiceProvider serviceProvider, ILogger<HourlyBackgroundService> logger)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                _logger.LogInformation("Updating VIP customers");
                UpdateVipCustomers(context);
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private void UpdateVipCustomers(DataContext context)
    {
        var vipCustomers = ( 
            from o in context.Orders
            join oi in context.OrderItems on o.OrderId equals oi.OrderId
            group oi by o.CustomerId into g
                where g.Select(oi => oi.ItemId).Distinct().Count() > 10 || 
                      g.Sum(oi => oi.ItemPrice * oi.Quantity) > 1e6
                      select g.Key).ToList();
        foreach (var customer in vipCustomers.Select(customerId => context.Customers.Find(customerId)).OfType<Customer>())
        {
            customer.IsVIP = 1;
        }

        context.SaveChanges();
    }
}