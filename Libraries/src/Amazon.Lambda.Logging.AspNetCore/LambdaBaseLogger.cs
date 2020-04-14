using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Amazon.Lambda.Logging.AspNetCore
{
    public abstract class LambdaBaseLogger : ILogger
    {
        protected readonly string _categoryName;
        protected readonly LambdaLoggerOptions _options;

        internal IExternalScopeProvider ScopeProvider { get; set; }

        // Constructor
        public LambdaBaseLogger(string categoryName, LambdaLoggerOptions options)
        {
            _categoryName = categoryName;
            _options = options;
        }


        // ILogger methods
        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? new NoOpDisposable();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_options.Filter == null || _options.Filter(_categoryName, logLevel));
        }

    }
}
