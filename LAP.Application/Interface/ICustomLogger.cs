namespace LAP.Application.Interface;

/// <summary>
/// Defines a comprehensive structured logging interface with support for multiple log levels.
/// </summary>
/// <typeparam name="T">The type of the class using the logger.</typeparam>
public interface ICustomLogger<T>
{
    /// <summary>Logs a verbose/trace level message.</summary>
    /// <param name="message">The log message.</param>
    void LogTrace(string message);

    /// <summary>Logs a verbose/trace level message with format parameters.</summary>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    void LogTrace(string message, params object?[] args);

    /// <summary>Logs a debug level message.</summary>
    /// <param name="message">The log message.</param>
    void LogDebug(string message);

    /// <summary>Logs a debug level message with format parameters.</summary>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    void LogDebug(string message, params object?[] args);

    /// <summary>Logs an informational message.</summary>
    /// <param name="message">The log message.</param>
    void LogInfo(string message);

    /// <summary>Logs an informational message with format parameters.</summary>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    void LogInfo(string message, params object?[] args);

    /// <summary>Logs a warning message.</summary>
    /// <param name="message">The log message.</param>
    void LogWarning(string message);

    /// <summary>Logs a warning message with format parameters.</summary>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    void LogWarning(string message, params object?[] args);

    /// <summary>Logs an error message.</summary>
    /// <param name="message">The log message.</param>
    void LogError(string message);

    /// <summary>Logs an error message with format parameters.</summary>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    void LogError(string message, params object?[] args);

    /// <summary>Logs an error message with an exception.</summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The log message.</param>
    void LogError(Exception exception, string message);

    /// <summary>Logs an error message with an exception and format parameters.</summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    void LogError(Exception exception, string message, params object?[] args);

    /// <summary>Logs a critical/fatal message.</summary>
    /// <param name="message">The log message.</param>
    void LogCritical(string message);

    /// <summary>Logs a critical/fatal message with format parameters.</summary>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    void LogCritical(string message, params object?[] args);

    /// <summary>Logs a critical/fatal message with an exception.</summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The log message.</param>
    void LogCritical(Exception exception, string message);

    /// <summary>Logs a critical/fatal message with an exception and format parameters.</summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The log message template.</param>
    /// <param name="args">Format arguments.</param>
    void LogCritical(Exception exception, string message, params object[] args);
}
