using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HotelListingAPI.Exceptions;
using Newtonsoft.Json;

namespace HotelListingAPI.Middleware
{
    public class ExceptionMiddleware
    {
        public RequestDelegate _next { get; }
        public ILogger<ExceptionMiddleware> _logger { get; }
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (System.Exception exception)
            {
                //log the exception and the request path
                _logger.LogError(exception, $"Something Went wrong while processing {context.Request.Path} ");
                
                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
           context.Response.ContentType = "application/json";

           HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
           var errorDetails = new ErrorDetails
           {
            ErrorType = "Failure",
            ErrorMessage = exception.Message,
           };

           switch (exception)
           {
            
            case NotFoundException notFoundException:
                 statusCode = HttpStatusCode.NotFound;
                 errorDetails.ErrorType = "Not Found";
                 break;
            default:
            break;     
           }

           string response = JsonConvert.SerializeObject(errorDetails);
           context.Response.StatusCode = (int)statusCode;
           return context.Response.WriteAsync(response);
        }
    }
}