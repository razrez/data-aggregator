
using Microsoft.Extensions.Logging;

namespace DataProvider.API.Middleware;

public class RequestResponseLoggingMiddleWare(ILogger logger) : IMiddleware
{
  private readonly ILogger _logger = logger;
  private long _reqCount = -1L;

  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    HttpRequest httpRequest = context.Request;
    long reqIndex = Interlocked.Increment(ref _reqCount);

    _logger.Log(
        LogLevel.Information,
        $"[{reqIndex}] --> **{httpRequest.Method}** __{httpRequest.Path}__"
    );

    try
    {
      await next.Invoke(context);
    }
    catch (Exception ex) 
    {
      _logger.LogError($"[{reqIndex}] --> **{httpRequest.Method}** __{httpRequest.Path}__ error occured: {ex.Message}");
      return;
    }

    _logger.Log(
        LogLevel.Information,
        $"[{reqIndex}] --> **{httpRequest.Method}** __{httpRequest.Path}__ {context.Response.StatusCode}"
    );

    throw new NotImplementedException();
  }
}
