# VictoriaLogs Logging Provider
Allows logging from your .NET app directly to [VictoriaLogs](https://docs.victoriametrics.com/victorialogs/)
## How to use
### Reference in your project

```
 dotnet add reference ../Dimaser.Dotnet.VictoriaLogs.csproj
```
or as a nuget from local nuget package feed
```
dotnet add package Dimaser.Dotnet.VictoriaLogs --source [/your-local/package-source]
```

### Add config
In your appsettings.json

```json
{
  "VictoriaLogsOptions": {
    "Endpoint": "http://localhost:9428/insert/jsonline",
    "AppName": "MyApp"
  }
}
```
`Endpoint`: required. URL to VictoriaLogs [HTTP JSON Stream APi](https://docs.victoriametrics.com/victorialogs/data-ingestion/#json-stream-api). If this is not provided or empty, logging will silently fail.

`AppName`: optional. Overrides the `app_name` field that gets passed to VictoriaLogs. If not provided, the default environment `ApplicationName` will be used.

### Register the logging provider
In your `Program.cs`
```c#
using Dimaser.Dotnet.VictoriaLogs;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddVictoriaLogs(
    builder.Configuration,
    builder.Environment);
```

The application will now log everything to VictoriaLogs. You can limit which log levels get logged by changing `LogLevel` under `Logging` in your `appsettings.json`. For example, if you only want to log `Error` and above, set:

```json
"Logging": {
    "LogLevel": {
      "Default": "Error",
      "Microsoft.AspNetCore": "Error"
    }
```
