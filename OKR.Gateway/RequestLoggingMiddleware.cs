using Serilog;
using System.Diagnostics;
using ILogger = Serilog.ILogger;

namespace OKR.Gateway
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var timestamp = DateTime.UtcNow;
                var errorMessage = "Request took longer than 500ms";
                var detail = $"Request {context.Request.Method} {context.Request.Path} took {elapsedMilliseconds} ms";

                Log.Warning("[{Timestamp}] [{ErrorMessage}] [{Detail}]",
                    timestamp, errorMessage, detail);
            }
        }
    }
}
