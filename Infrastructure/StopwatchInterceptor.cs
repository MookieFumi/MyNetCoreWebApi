using System.Diagnostics;
using System.Linq;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace mywebapi.Infrastructure
{
    public class StopwatchInterceptor : IInterceptor
    {
        private readonly ILoggerFactory _loggerFactory;

        public StopwatchInterceptor(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        
        public void Intercept(IInvocation invocation)
        {
            var logger = _loggerFactory.CreateLogger(invocation.TargetType);
            var stopwatch = new Stopwatch();
            
            stopwatch.Start();
            logger.LogInformation($"{invocation.TargetType}.{invocation.Method.Name} Params {string.Join(", ", invocation.Arguments.Select(a => (JsonConvert.SerializeObject(a) ?? "").ToString()).ToArray())}");

            invocation.Proceed();

            stopwatch.Stop();
            logger.LogInformation($"Done ({stopwatch.ElapsedMilliseconds} ms): result was {JsonConvert.SerializeObject(invocation.ReturnValue)}.");
        }
    }
}