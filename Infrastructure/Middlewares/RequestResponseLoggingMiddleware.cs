using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;

namespace mywebapi.Infrastructure.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next,
                                                ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory
                      .CreateLogger<RequestResponseLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("company"))
            {
                await _next(context);
            }
            else
            {
                context.Request.Headers.TryGetValue("company", out var company);

                _logger.LogInformation(await FormatRequest(company, context.Request));

                var originalBodyStream = context.Response.Body;

                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    _logger.LogInformation(await FormatResponse(company, context.Response));
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
        }

        private async Task<string> FormatRequest(string company, HttpRequest request)
        {
            var body = request.Body;
            request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body = body;

            return $"Request {company} --> {request.Scheme}://{request.Host}{request.Path} {request.QueryString} {bodyAsText}[Length: {buffer.Length}]";
        }

        private async Task<string> FormatResponse(string company, HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"Response {company} --> {text} [Length: {response.Body.Length}]";
        }
    }
}