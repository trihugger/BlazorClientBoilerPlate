using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace BlazorClientBoilerPlate.CoreApi
{
    public class CustomLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, CustomLogger> _loggers 
            = new ConcurrentDictionary<string, CustomLogger>();

        public CustomLoggerProvider()
        {

        }

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new(name));

        public void Dispose() => _loggers.Clear();
    }
}
