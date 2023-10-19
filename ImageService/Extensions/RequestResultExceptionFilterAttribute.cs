using System.Security.Authentication;
using ImageService.Models;
using ImageService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ImageService.Extensions;

public class RequestResultExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        context.ExceptionHandled = true;

        var exceptionMessage = context.Exception.Message;

        var log = context.HttpContext.RequestServices.GetService<ILogger>();
        log?.LogError(exceptionMessage);

        int statusCode = context.Exception switch
        {
            AuthenticationException => 401,
            StorageException => 500,
            _ => 400
        };

        context.Result = new JsonResult(new ErrorResult(context.Exception.GetType().Name, exceptionMessage))
        {
            StatusCode = statusCode
        };
    }
}