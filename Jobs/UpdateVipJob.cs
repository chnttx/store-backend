using Quartz;
using WebApplication2.Services;
using WebApplication2.Services.Interface;

namespace WebApplication2.Jobs;

public class UpdateVipJob : IJob
{
    private readonly ICustomerService _customerService;

    public UpdateVipJob(ICustomerService customerService)
    {
        _customerService = customerService;
    }


    public async Task Execute(IJobExecutionContext context)
    {
        await _customerService.UpdateVipCustomers();
    }
}