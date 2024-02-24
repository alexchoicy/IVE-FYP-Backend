using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.utils;
using Newtonsoft.Json;
using Serilog;

namespace api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error(ex.Message);
                var response = new ApiResponse<object>
                {
                    Data = null,
                    ErrorMessage = "Internal Server Error",
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Success = false
                };

                var jsonResponse = JsonConvert.SerializeObject(response);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(jsonResponse);
            }
        }

    }
}