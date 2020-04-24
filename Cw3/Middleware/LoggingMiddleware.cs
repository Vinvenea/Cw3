using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Cw3.Middleware
{
   
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            string wholeline = "";
            wholeline += httpContext.Request.Method.ToString() + " ";
            wholeline += httpContext.Request.Path.ToString() + " ";
            var bodySerialized = string.Empty;

            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1824, true))
            {
                bodySerialized = await reader.ReadToEndAsync();
            }
            wholeline += bodySerialized;
            wholeline += httpContext.Request.QueryString.ToString();
            var startupPath = @Environment.CurrentDirectory + @"\logRequest.txt";

            using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(@startupPath, true))

            {
                file.WriteLine(wholeline);
            }
          
            await _next(httpContext);
        }
    }
}
