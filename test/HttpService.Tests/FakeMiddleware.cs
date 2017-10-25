using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HttpService.Tests
{
    public class FakeMiddleware
    {
        private TimeSpan _delay;
        private RequestDelegate _next;

        private Microsoft.Extensions.Logging.ILogger _logger;

        public FakeMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, TimeSpan delay)
        {
            this._next = next;
            this._delay = delay;
            this._logger = loggerFactory.CreateLogger("fake-middleware");
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("FakeMiddleware has been called");

            System.Threading.Thread.Sleep(_delay);

            if (context.Request.Path.StartsWithSegments("/throw"))
            {
                throw new InvalidOperationException("expected exception");
            }
            await _next(context);
        }

    }
}