using LAP.Application.Interface;
using Microsoft.Extensions.Logging;

namespace LAP.Infrastructure.Logging;

/// <summary>
/// Wraps Microsoft.Extensions.Logging.ILogger to provide structured logging at all standard log levels.
/// </summary>
/// <typeparam name="T">The type of the class using the logger.</typeparam>
public class CustomLogger<T> : ICustomLogger<T>
{
    private readonly ILogger<T> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomLogger{T}"/> class.
    /// </summary>
    /// <param name="logger">The underlying logger.</param>
    public CustomLogger(ILogger<T> logger)
    {
        _logger = logger;
    }

    /// <summary>Logs a verbose/trace level message.</summary>
    /// <param name="message">The log message.</param>
    public void LogTrace(string message)
    {
        _logger.LogTrace(message);
    }

    /// <summary>Logs a verbose/trace level message with format parameters.</summary>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    public void LogTrace(string message, params object?[] args)
    {
        _logger.LogTrace(message, args);
    }

    /// <summary>Logs a debug level message.</summary>
    /// <param name="message">The log message.</param>
    public void LogDebug(string message)
    {
        _logger.LogDebug(message);
    }

    /// <summary>Logs a debug level message with format parameters.</summary>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    public void LogDebug(string message, params object?[] args)
    {
        _logger.LogDebug(message, args);
    }

    /// <summary>Logs an informational message.</summary>
    /// <param name="message">The log message.</param>
    public void LogInfo(string message)
    {
        _logger.LogInformation(message);
    }

    /// <summary>Logs an informational message with format parameters.</summary>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    public void LogInfo(string message, params object?[] args)
    {
        _logger.LogInformation(message, args);
    }

    /// <summary>Logs a warning message.</summary>
    /// <param name="message">The log message.</param>
    public void LogWarning(string message)
    {
        _logger.LogWarning(message);
    }

    /// <summary>Logs a warning message with format parameters.</summary>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    public void LogWarning(string message, params object?[] args)
    {
        _logger.LogWarning(message, args);
    }

    /// <summary>Logs an error message.</summary>
    /// <param name="message">The log message.</param>
    public void LogError(string message)
    {
        _logger.LogError(message);
    }

    /// <summary>Logs an error message with format parameters.</summary>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    public void LogError(string message, params object?[] args)
    {
        _logger.LogError(message, args);
    }

    /// <summary>Logs an error message with an exception.</summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The log message.</param>
    public void LogError(Exception exception, string message)
    {
        _logger.LogError(exception, message);
    }

    /// <summary>Logs an error message with an exception and format parameters.</summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    public void LogError(Exception exception, string message, params object?[] args)
    {
        _logger.LogError(exception, message, args);
    }

    /// <summary>Logs a critical/fatal message.</summary>
    /// <param name="message">The log message.</param>
    public void LogCritical(string message)
    {
        _logger.LogCritical(message);
    }

    /// <summary>Logs a critical/fatal message with format parameters.</summary>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    public void LogCritical(string message, params object?[] args)
    {
        _logger.LogCritical(message, args);
    }

    /// <summary>Logs a critical/fatal message with an exception.</summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The log message.</param>
    public void LogCritical(Exception exception, string message)
    {
        _logger.LogCritical(exception, message);
    }

    /// <summary>Logs a critical/fatal message with an exception and format parameters.</summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    public void LogCritical(Exception exception, string message, params object?[] args)
    {
        _logger.LogCritical(exception, message, args);
    }
}
