using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace GistSync.NET.Utils
{
    public class ActivityLogger : ILogger
    {
        private readonly static IList<(LogLevel logLevel, string logMessage)> BufferMessage = new List<(LogLevel, string)>();
        private static RichTextBox? _connectedRichTextBox;
        private static SynchronizationContext? _syncContext;

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) => logLevel switch
        {
            LogLevel.Critical => true,
            LogLevel.Error => true,
            LogLevel.Warning => true,
            LogLevel.Information => true,
            _ => false
        };

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var logMessage = $"{logLevel} ({DateTime.Now:yyyy/MM/dd-HH:mm:ss}): {formatter(state, exception)}\n";

            if (_connectedRichTextBox is null)
                BufferMessage.Add((logLevel, logMessage)); // Add to buffer if the rich text box is not ready.
            else
                PrintMessage(logLevel, logMessage);
        }

        public static void Connect(SynchronizationContext syncContext, RichTextBox richTextBox)
        {
            _syncContext = syncContext;
            _connectedRichTextBox = richTextBox;

            // Once connected, flush the buffered log messages
            foreach (var (logLevel, logMessage) in BufferMessage)
                PrintMessage(logLevel, logMessage);

            BufferMessage.Clear();
        }

        public static Color GetColorByLogLevel(LogLevel logLevel) => logLevel switch
        {
            LogLevel.Critical => Color.DarkRed,
            LogLevel.Error => Color.Red,
            LogLevel.Warning => Color.Yellow,
            _ => Color.Black
        };

        private static void PrintMessage(LogLevel logLevel, string logMessage)
        {
            if (_connectedRichTextBox == null) return;

            if (_syncContext == null)
            {
                _connectedRichTextBox.ForeColor = GetColorByLogLevel(logLevel);
                _connectedRichTextBox.AppendText(logMessage);
            }
            else
                _syncContext.Send(s =>
                {
                    _connectedRichTextBox.ForeColor = GetColorByLogLevel(logLevel);
                    _connectedRichTextBox.AppendText(logMessage);
                }, null);
        }
    }

    public class ActivityLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ActivityLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

        public void Dispose()
        {
            _loggers.Clear();
        }

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new ActivityLogger());
    }

    public static class ActivityLoggerExtensions
    {
        public static ILoggingBuilder AddActivityLogger(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, ActivityLoggerProvider>());

            return builder;
        }
    }
}
