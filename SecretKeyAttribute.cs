using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication2;

public class SecretKeyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("secret_key", out var secretKey) ||
            secretKey != "123456")
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
        
        base.OnActionExecuting(context);
    }
}