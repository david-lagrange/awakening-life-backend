using Serilog.Sinks.AwsCloudWatch;

namespace AwakeningLifeBackend.Providers;

public class DailyLogStreamNameProvider : ILogStreamNameProvider
{
    public string GetLogStreamName()
    {
        var deployedEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (string.IsNullOrEmpty(deployedEnv))
        {
            deployedEnv = "default";
        }

        return $"{deployedEnv.ToLower()}-environment-{DateTime.UtcNow:yyyy-MM-dd}";
    }
}