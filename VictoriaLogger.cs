using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Dimaser.Dotnet.VictoriaLogs;

public class VictoriaLogger : ILogger
{
    private readonly string _category;
    private readonly VictoriaLogsOptions _options;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _contextAccessor;
    public VictoriaLogger(string category, VictoriaLogsOptions options, HttpClient httpClient, IHttpContextAccessor contextAccessor)
    {
        _category = category;
        _options = options;
        _httpClient = httpClient;
        _contextAccessor = contextAccessor;
        _httpClient.DefaultRequestHeaders.Add("VL-Stream-Fields", "hostname,app_name");
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        _ = SendAsync(logLevel, formatter(state, exception), exception);
    }

    private async Task SendAsync(LogLevel logLevel, string message, Exception? exception)
    {

        var context = _contextAccessor.HttpContext;
        var request = context?.Request;

        var logEntry = new
            {

                _time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                _msg = exception != null ? exception.Message : message,
                level = logLevel.ToString().ToLower(),
                category = _category,
                hostname = Environment.MachineName,
                app_name = _options.AppName,
                remote_ip = context?.Connection?.RemoteIpAddress?.ToString(),
                path = request?.Path.Value,
                query = request?.QueryString.Value,
                method = request?.Method,
                user_id = context?.User?.Identity?.IsAuthenticated == true ? context.User.Identity.Name : null,
                user_agent = request?.Headers["User-Agent"].ToString(),
                ex = exception == null ? null : new
                {
                    type = exception.GetType().FullName,
                    stacktrace = exception.ToString()
                }
            };

        var json = JsonSerializer.Serialize(logEntry);

        var content = new StringContent(json + "\n", Encoding.UTF8, "application/x-ndjson");

        try
        {
            if (!string.IsNullOrWhiteSpace(_options.Endpoint))
            {
                await _httpClient.PostAsync(_options.Endpoint, content);
            }
        }
        catch
        {
        }
    }
}
