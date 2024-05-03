using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace WebApplication1
{
    public class ApplicationNameMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _applicationName;

        public ApplicationNameMiddleware(RequestDelegate next, string applicationName)
        {
            _next = next;
            _applicationName = applicationName;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                context.Response.ContentType = "application/json";
                var applicationInfo = new { ApplicationName = _applicationName };
                await context.Response.WriteAsync(JsonConvert.SerializeObject(applicationInfo));
            }
            else
            {
                await _next(context);
            }
        }
    }
}
