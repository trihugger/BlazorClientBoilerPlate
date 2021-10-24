namespace BlazorClientBoilerPlate.CoreApi
{
    public class CustomLogger : ILogger
    {
        private readonly string _name;

        public CustomLogger(string name)
        {
            _name = name;
        }

        public IDisposable BeginScope<TState>(TState state) => default; // TODO: set the construct for this

        public bool IsEnabled(LogLevel logLevel) => true; // TODO: add logic based on the minimal level set

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Console.WriteLine($"[{eventId,2}: {logLevel,-12}] {_name} - {state}" + (exception != null ? $" {exception.Message}." : ".")); // TODO: finish this
        }
    }
}