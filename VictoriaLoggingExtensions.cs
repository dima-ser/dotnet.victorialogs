using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Dimaser.Dotnet.VictoriaLogs;

public static class VictoriaLoggingExtensions
{
    public static ILoggingBuilder AddVictoriaLogs(this ILoggingBuilder builder,
        IConfiguration config,
        IHostEnvironment env)
    {
        var options = config
            .GetSection(nameof(VictoriaLogsOptions))
            .Get<VictoriaLogsOptions>() ?? new VictoriaLogsOptions();

        if (string.IsNullOrWhiteSpace(options.AppName))
        {
            options.AppName = env.ApplicationName;
        }

        
 // ✅ Ensure accessor is available
        builder.Services.AddHttpContextAccessor();

        if (!string.IsNullOrWhiteSpace(options.Endpoint))
        {
            builder.Services.AddSingleton<ILoggerProvider>(sp =>
                new VictoriaLoggerProvider(
                    options,
                    sp.GetRequiredService<IHttpContextAccessor>()
                ));
        }

        return builder;
    }
}
