using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MicroNetCore.AspNetCore.Proxy.Middleware
{
    /// <summary>
    ///     Captures HTTP requests and redirects them to given target destination if found in ProxyOptions. If not found
    ///     continues HTTP pipeline.
    /// </summary>
    public sealed class ProxyMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private readonly IProxy _proxy;

        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="T:MicroNetCore.AspNetCore.Proxy.Middleware.ProxyMiddleware" /> class.
        /// </summary>
        /// <param name="next">Next <see cref="T:Microsoft.AspNetCore.Http.RequestDelegate" /> in HTTP pipeline.</param>
        /// <param name="loggerFactory"><see cref="T:Microsoft.Extensions.Logging.ILoggerFactory" /> for logging.</param>
        /// <param name="proxy"><see cref="T:MicroNetCore.AspNetCore.Proxy.IProxy" /> for resolving proxy target destinations.</param>
        public ProxyMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IProxy proxy)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ProxyMiddleware>();
            _proxy = proxy;
        }

        /// <summary>
        ///     Process an individual request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var request = CreateRequestMessage(context.Request);
                await ExecuteRequest(request, context);
            }
            catch (Exception e)
            {
                await _next(context);
                _logger.LogError(e.ToString());
            }
        }

        private HttpRequestMessage CreateRequestMessage(HttpRequest request)
        {
            var message = new HttpRequestMessage
            {
                Method = new HttpMethod(request.Method),
                RequestUri = new Uri(_proxy.Map(request.Path))
            };

            CopyRequestHeaders(request, message);
            CopyRequestBody(request, message);

            return message;
        }

        private static void CopyRequestHeaders(HttpRequest original, HttpRequestMessage copy)
        {
            foreach (var header in original.Headers)
            {
                if (header.Key == HttpRequestHeader.Authorization.ToString())
                    continue;

                if (!copy.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                    copy.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        private static void CopyRequestBody(HttpRequest original, HttpRequestMessage copy)
        {
            if (!HasBody(original.Method))
                return;

            copy.Content = new StreamContent(original.Body);
        }

        private static bool HasBody(string method)
        {
            return !new[] {"GET", "HEAD", "DELETE", "TRACE"}
                .Any(m => string.Equals(method, m, StringComparison.OrdinalIgnoreCase));
        }

        private static async Task ExecuteRequest(HttpRequestMessage message, HttpContext context)
        {
            using (var response = await GetResponse(message, context.RequestAborted))
            {
                CopyStatusCode(response, context.Response);
                CopyResponseHeaders(response, context.Response);

                await CopyResponseBody(response, context.Response);
            }
        }

        private static async Task<HttpResponseMessage> GetResponse(
            HttpRequestMessage message, CancellationToken cancellation)
        {
            using (var client = new HttpClient(new HttpClientHandler()))
            {
                return await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, cancellation);
            }
        }

        private static void CopyStatusCode(HttpResponseMessage original, HttpResponse copy)
        {
            copy.StatusCode = (int) original.StatusCode;
        }

        private static void CopyResponseHeaders(HttpResponseMessage original, HttpResponse copy)
        {
            foreach (var header in original.Headers)
                copy.Headers[header.Key] = header.Value.ToArray();

            foreach (var header in original.Content.Headers)
                copy.Headers[header.Key] = header.Value.ToArray();

            copy.Headers.Remove("transfer-encoding");
        }

        private static async Task CopyResponseBody(HttpResponseMessage original, HttpResponse copy)
        {
            await original.Content.CopyToAsync(copy.Body);
        }
    }
}