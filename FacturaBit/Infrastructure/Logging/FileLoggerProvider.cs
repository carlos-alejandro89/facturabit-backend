using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace FacturaBit.Infrastructure.Logging;

public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _logsDirectory;
    private readonly object _writeLock = new();
    private readonly ConcurrentDictionary<string, FileLogger> _loggers = new();

    public FileLoggerProvider(string logsDirectory)
    {
        _logsDirectory = logsDirectory;
        Directory.CreateDirectory(_logsDirectory);
    }

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, category => new FileLogger(category, Write));

    public void Dispose()
    {
        _loggers.Clear();
    }

    private void Write(
        string category,
        LogLevel logLevel,
        EventId eventId,
        string message,
        Exception? exception)
    {
        try
        {
            string path = Path.Combine(
                _logsDirectory,
                $"carti-{DateTimeOffset.Now:yyyy-MM-dd}.log");
            string exceptionText = exception is null
                ? string.Empty
                : $"{Environment.NewLine}{exception}";
            string entry =
                $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz} " +
                $"[{logLevel}] [{category}] [{eventId.Id}] {message}" +
                $"{exceptionText}{Environment.NewLine}";

            lock (_writeLock)
            {
                File.AppendAllText(path, entry);
            }
        }
        catch
        {
            // Un error al escribir el log nunca debe interrumpir la operación principal.
        }
    }

    private sealed class FileLogger : ILogger
    {
        private readonly string _category;
        private readonly Action<string, LogLevel, EventId, string, Exception?> _write;

        public FileLogger(
            string category,
            Action<string, LogLevel, EventId, string, Exception?> write)
        {
            _category = category;
            _write = write;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            _write(_category, logLevel, eventId, formatter(state, exception), exception);
        }
    }
}
