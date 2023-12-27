using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Models;

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
