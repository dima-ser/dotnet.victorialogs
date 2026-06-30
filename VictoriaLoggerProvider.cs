using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Dimaser.VictoriaLogs.Dotnet;

public class VictoriaLoggerProvider : ILoggerProvider
{
    private readonly VictoriaLogsOptions _options;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public VictoriaLoggerProvider(VictoriaLogsOptions options, IHttpContextAccessor httpContextAccessor)
    {
        _options = options;
        _httpClient = new HttpClient();
        _httpContextAccessor = httpContextAccessor;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new VictoriaLogger(categoryName, _options, _httpClient, _httpContextAccessor);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
