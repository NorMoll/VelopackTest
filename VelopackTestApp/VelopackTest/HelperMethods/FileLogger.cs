using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace VelopackTest
{
    public class FileLogger : ILogger
    {
        private string filePath;
        private static object _lock = new object();

        public FileLogger(string path)
        {
            filePath = path;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            lock (_lock)
            {
                File.AppendAllText(filePath, DateTime.Now.ToString("f") + "; " + formatter(state, exception) + Environment.NewLine);
            }
        }
    }
}
