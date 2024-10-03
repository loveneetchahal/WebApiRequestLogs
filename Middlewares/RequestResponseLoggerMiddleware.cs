using Azure.Core;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WebApiRequestLogs.DbContexts;
using WebApiRequestLogs.Models;
using WebApiRequestLogs.Services;

namespace WebApiRequestLogs.Middlewares
{
    public class RequestResponseLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly RequestResponseLoggerOption _options; 
        private readonly ILogger<RequestResponseLoggerMiddleware> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RequestLogs _requestLogs;
        public RequestResponseLoggerMiddleware(RequestDelegate next, 
            //IOptions<RequestResponseLoggerOption> options,
            ILogger<RequestResponseLoggerMiddleware> logger, IServiceScopeFactory serviceScopeFactory, RequestLogs requestLogs)
        {
            _next = next;
            //_options = options.Value;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _requestLogs = requestLogs;
        }

        public async Task InvokeAsync(HttpContext httpContext)
            //, IRequestResponseLogModelCreator logCreator)
        {
            RequestResponseLogModel log = new RequestResponseLogModel();//logCreator.LogModel;
            // Middleware is enabled only when the EnableRequestResponseLogging config value is set.
            //if (_options == null || !_options.IsEnabled)
            //{
            //    await _next(httpContext);
            //    return;
            //}
            log.RequestDateTimeUtc = DateTime.UtcNow;
            HttpRequest request = httpContext.Request;

            /*log*/
            log.LogId = Guid.NewGuid().ToString();
            log.TraceId = httpContext.TraceIdentifier;
            var ip = request.HttpContext.Connection.RemoteIpAddress;
            log.ClientIp = ip == null ? null : ip.ToString();
            //log.Node = _options.Name;

            /*request*/
            log.RequestMethod = request.Method;
            log.RequestPath = request.Path;
            log.RequestQuery = request.QueryString.ToString();
            log.RequestQueries = FormatQueries(request.QueryString.ToString());
            log.RequestHeaders = FormatHeaders(request.Headers);
            log.RequestBody = await ReadBodyFromRequest(request);
            log.RequestScheme = request.Scheme;
            log.RequestHost = request.Host.ToString();
            log.RequestContentType = request.ContentType; 
            // Temporarily replace the HttpResponseStream, which is a write-only stream, with a MemoryStream to capture it's value in-flight.
            HttpResponse response = httpContext.Response;
            var originalResponseBody = response.Body;
            using var newResponseBody = new MemoryStream();
            response.Body = newResponseBody;

            // Call the next middleware in the pipeline
            try
            {
                await _next(httpContext);
            }
            catch (Exception exception)
            {
                /*exception: but was not managed at app.UseExceptionHandler() or by any middleware*/
                LogError(log, exception);
            }

            newResponseBody.Seek(0, SeekOrigin.Begin);
            var responseBodyText = await new StreamReader(response.Body).ReadToEndAsync();

            newResponseBody.Seek(0, SeekOrigin.Begin);
            await newResponseBody.CopyToAsync(originalResponseBody); 
            /*response*/
            log.ResponseContentType = response.ContentType;
            log.ResponseStatus = response.StatusCode.ToString();
            log.ResponseHeaders = FormatHeaders(response.Headers);
            log.ResponseBody = responseBodyText;
            log.ResponseDateTimeUtc = DateTime.UtcNow; 
            var contextFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (contextFeature != null && contextFeature.Error != null)
            {
                Exception exception = contextFeature.Error;
                LogError(log, exception);
            }

            SaveLogInDb(log);
        } 
        public void SaveLogInDb(RequestResponseLogModel log)
        {
            RequestLogs requestLogs = new RequestLogs();
            requestLogs.LogId = Guid.NewGuid().ToString();
            requestLogs.TraceId = log.TraceId;
            requestLogs.ClientIp = log.ClientIp;
            requestLogs.Node = log.Node;
            requestLogs.RequestMethod = log.RequestMethod.ToString();
            requestLogs.RequestPath = log.RequestPath.ToString();
            requestLogs.RequestQuery = log.RequestQuery.ToString();
            requestLogs.RequestQueries = log.RequestQueries.ToString();
            requestLogs.RequestHeaders = log.RequestHeaders.ToString();
            requestLogs.RequestBody = log.RequestBody.ToString();
            requestLogs.RequestScheme = log.RequestScheme.ToString();
            requestLogs.RequestHost = log.RequestHost.ToString();
            requestLogs.RequestContentType = log.RequestContentType;
            requestLogs.ResponseContentType = log.ResponseContentType;
            requestLogs.ResponseStatus = log.ResponseStatus.ToString();
            requestLogs.ResponseHeaders = log.ResponseHeaders.ToString();
            requestLogs.ResponseBody = log.ResponseBody.ToString();
            requestLogs.ResponseDateTimeUtc = DateTime.UtcNow;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                // Use dbContext
                _appDbContext.RequestLogs.Add(requestLogs);
                _appDbContext.SaveChanges();
            }
        }

        private void LogError(RequestResponseLogModel log, Exception exception)
        {
            log.ExceptionMessage = exception.Message;
            log.ExceptionStackTrace = exception.StackTrace;
        }

        private Dictionary<string, string> FormatHeaders(IHeaderDictionary headers)
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            foreach (var header in headers)
            {
                pairs.Add(header.Key, header.Value);
            }
            return pairs;
        }

        private List<KeyValuePair<string, string>> FormatQueries(string queryString)
        {
            List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
            string key, value;
            foreach (var query in queryString.TrimStart('?').Split("&"))
            {
                var items = query.Split("=");
                key = items.Count() >= 1 ? items[0] : string.Empty;
                value = items.Count() >= 2 ? items[1] : string.Empty;
                if (!String.IsNullOrEmpty(key))
                {
                    pairs.Add(new KeyValuePair<string, string>(key, value));
                }
            }
            return pairs;
        }

        private async Task<string> ReadBodyFromRequest(HttpRequest request)
        {
            // Ensure the request's body can be read multiple times (for the next middlewares in the pipeline).
            request.EnableBuffering();
            using var streamReader = new StreamReader(request.Body, leaveOpen: true);
            var requestBody = await streamReader.ReadToEndAsync();
            // Reset the request's body stream position for next middleware in the pipeline.
            request.Body.Position = 0;
            return requestBody;
        }

        public string DictionaryToString(Dictionary<string, string> dictionary)
        {
            string dictionaryString = "{";
            foreach (KeyValuePair<string, string> keyValues in dictionary)
            {
                dictionaryString += keyValues.Key + " : " + keyValues.Value + ", ";
            }
            return dictionaryString.TrimEnd(',', ' ') + "}";
        }
    }
}
