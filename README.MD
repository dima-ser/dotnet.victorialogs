# VictoriaLogs .NET Logging Provider
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

The application will now log everything to VictoriaLogs. You can limit which [log levels](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/overview?tabs=command-line#log-level) get logged by changing `LogLevel` under `Logging` in your `appsettings.json`. For example, if you only want to log `Error` and above, set:

```json
"Logging": {
    "LogLevel": {
      "Default": "Error",
      "Microsoft.AspNetCore": "Error"
    }
```
## Log fields
The library will log the following fields to VictoriaLogs. `_time` and `_msg`  are required by VictoriaLogs. 

| Field name  | Description |
| ----------- | ----------- |
| _time       | Log timestamp in UTC. |
| _msg        | Log message. In case of exceptions, this will contain `Exception.Message` |
| _stream     | Combination of `hostname` and `app_name` to uniquely identify application as per [VictoriaLogs docs](https://docs.victoriametrics.com/victorialogs/keyconcepts/#stream-fields). This reduces disk space usage and improves search performance in VictoriaLogs.
| level       | Log level  as defined in https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/overview?tabs=command-line#log-level |
| category    | Log category as defined in https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/overview?tabs=command-line#log-category |
| hostname    | Machine name from `Environment.MachineName` |
| app_name    | `AppName` as specified in configuration or `Environment.ApplicationName` |
| remote_ip   | IP address of the current request |
| path        | Path of the current web request |
| query       | Query string of the current request (if any) |
| method      | Current request HTTP method (GET, POST, etc) |
| user_id     | Currently authenticated user (if any) |
| user_agent  | User agent of the current request |
| ex.type     | Exception type (if any) |
| ex.stacktrace | Stack trace (if any) |