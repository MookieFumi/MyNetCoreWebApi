using System.Diagnostics;
using System.Linq;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace mywebapi.Infrastructure
{
    public class CallLoggerInterceptor : IInterceptor
    {
        private readonly ILogger<CallLoggerInterceptor> _logger;

        public CallLoggerInterceptor(ILoggerFactory loggerFactory)
        {
            _logger = new Logger<CallLoggerInterceptor>(loggerFactory);
        }

        public void Intercept(IInvocation invocation)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _logger.LogInformation($"{invocation.TargetType}.{invocation.Method.Name} Params {string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())}");

            invocation.Proceed();

            stopwatch.Stop();
            _logger.LogInformation($"Done ({stopwatch.ElapsedMilliseconds} ms): result was {invocation.ReturnValue}.");
        }
    }
}