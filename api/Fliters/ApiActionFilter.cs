using api.Exceptions;
using api.utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
namespace api.Fliters
{
    public class ApiActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            string? ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            string endpoint = context.HttpContext.Request.Path;
            string? action = context.ActionDescriptor.DisplayName;
            string? method = context.HttpContext.Request.Method;

            if (context.Result is ObjectResult objectResult)
            {
                int? statusCode = objectResult.StatusCode;
                ApiResponse<object> response = new ApiResponse<object>
                {
                    StatusCode = statusCode ?? StatusCodes.Status200OK,
                };
                if (objectResult.Value is BaseCustomExceptions customExceptions)
                {
                    response.Success = false;
                    response.ErrorMessage = customExceptions.Message;
                    Log.Error($"Failed: Request from {ip} to {endpoint}, {method}, {action}, {customExceptions.GetType().Name}");
                }
                else if (objectResult.Value is Exception exception)
                {
                    response.Success = false;
                    response.ErrorMessage = "Internal Server Error";
                    Log.Error($"Failed: Request from {ip} to {endpoint}, {method}, {action}, {exception.GetType().Name}");
                }
                else
                {
                    response.Success = true;
                    response.Data = objectResult.Value;
                    Log.Information($"Success: Request from {ip} to {endpoint}, {method}, {action}");
                }
                context.Result = new ObjectResult(response)
                {
                    StatusCode = objectResult.StatusCode
                };
            }
        }


    }
}