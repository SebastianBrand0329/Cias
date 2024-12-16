using NLog;

namespace ApiCursosCias.Services.Helpers.Middleware;

public class LoggerManager
{
    public static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public static void LogDebug(string message)
    {
        logger.Debug($"{message}");
    }

    public static void LogError(string message)
    {
        logger.Error($"{message}");
    }

    public static void LogInfo(string message)
    {
        logger.Info($"{message}");
    }

    public static void LogWarn(string message)
    {
        logger.Warn($"{message}");
    }
}