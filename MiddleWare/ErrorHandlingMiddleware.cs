using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace IEEEOUIparser.MiddleWare
{
    /// <summary>
    /// Custom Middleware that 'supposed' to capture all unhandled exceptions.
    /// https://blog.jonblankenship.com/2020/04/12/global-exception-handling-in-aspnet-core-api/
    /// </summary>
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            const HttpStatusCode status = HttpStatusCode.InternalServerError;
            string Message = exception.Message;
            var StackTrace = exception.StackTrace;
            var ExceptionType = exception.GetType();

            const string result = "";

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(result);
        }
    }
}