using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace mywebapi.Infrastructure
{
    /// <summary>
    /// https://github.com/JSkimming/Castle.Core.AsyncInterceptor
    /// </summary>
    public class StopwatchAsyncInterceptor : IAsyncInterceptor
    {
        private readonly ILoggerFactory _loggerFactory;

        public StopwatchAsyncInterceptor(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            var stopwatch = Stopwatch.StartNew();
            LogBeforeProceed(invocation);

            invocation.Proceed();

            stopwatch.Stop();
            LogAfterProceed(invocation, stopwatch.ElapsedMilliseconds, invocation.ReturnValue);
        }

        private void LogBeforeProceed(IInvocation invocation)
        {
            var logger = _loggerFactory.CreateLogger(invocation.TargetType);
            logger.LogInformation($"{nameof(StopwatchAsyncInterceptor)} --> Before --> {invocation.TargetType}.{invocation.Method.Name} Params {string.Join(", ", invocation.Arguments.Select(a => (JsonConvert.SerializeObject(a) ?? "").ToString()).ToArray())}", invocation.Arguments);
        }

        private void LogAfterProceed(IInvocation invocation, long elapsedMilliseconds, object result)
        {
            var logger = _loggerFactory.CreateLogger(invocation.TargetType.ToString());
            logger.LogInformation($"{nameof(StopwatchAsyncInterceptor)} --> After --> {invocation.TargetType}.{invocation.Method.Name} ({elapsedMilliseconds} ms): result was {JsonConvert.SerializeObject(result)}.", result);
        }

        private async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            // Step 1. Do something prior to invocation.
            var stopwatch = Stopwatch.StartNew();
            LogBeforeProceed(invocation);

            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            await task;

            // Step 2. Do something after invocation.
            stopwatch.Stop();
            LogAfterProceed(invocation, stopwatch.ElapsedMilliseconds, new { });
        }

        private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            // Step 1. Do something prior to invocation.
            var stopwatch = Stopwatch.StartNew();
            LogBeforeProceed(invocation);

            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            var result = await task;

            // Step 2. Do something after invocation.
            stopwatch.Stop();
            LogAfterProceed(invocation, stopwatch.ElapsedMilliseconds, result);

            return result;
        }
    }
}