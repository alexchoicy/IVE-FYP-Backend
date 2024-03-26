using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace api.Middleware
{
    public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new AuthorizationMiddlewareResultHandler();

        public async Task HandleAsync(
            RequestDelegate requestDelegate,
            HttpContext httpContext,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            // If authorization was successful, continue with the pipeline
            if (authorizeResult.Succeeded)
            {
                await requestDelegate(httpContext);
                return;
            }

            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new ApiResponse<string>
            {
                ErrorMessage = "You are not Authorized, MR.TOKEN HAVE ISSUES",
                Success = false
            });
        }
    }
}