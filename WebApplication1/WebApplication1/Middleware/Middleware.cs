using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using businesslogic.Exceptions;

namespace WebApplication1.Middleware
{
    // Middleware para manejar excepciones globales
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError; // Por defecto, se asume un error interno del servidor

            if (ex is MyException)
            {
                statusCode = HttpStatusCode.BadRequest; // Cambiar a un código de estado adecuado según la excepción
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            // Construir el cuerpo de la respuesta JSON
            string responseJson = $"{{ \"message\": \"{ex.Message}\" }}";

            return context.Response.WriteAsync(responseJson);
        }
    }

    // Clase de extensión para agregar el middleware al pipeline de solicitudes HTTP
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
