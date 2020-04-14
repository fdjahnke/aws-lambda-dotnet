using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Amazon.Lambda.Logging.AspNetCore
{
    public class LambdaJsonLogger : LambdaBaseLogger
    {
        // Constructor
        public LambdaJsonLogger(string categoryName, LambdaLoggerOptions options) : base(categoryName,options){}

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            if (!IsEnabled(logLevel))
            {
                return;
            }

            var logEntry = new LogEntry();

            if (_options.IncludeLogLevel)
            {
                logEntry.LogLevel = logLevel;
            }

            CreateScopeInformation(logEntry);

            if (_options.IncludeCategory)
            {
                logEntry.Category = _categoryName;
            }

            if (_options.IncludeEventId)
            {
                logEntry.EventId = eventId;
            }

            var text = formatter.Invoke(state, exception);
            logEntry.Text = text;

            if (_options.IncludeException)
            {
                logEntry.Exception = exception;
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize<LogEntry>(logEntry, options);
            Amazon.Lambda.Core.LambdaLogger.Log(json);
        }

        private void CreateScopeInformation(LogEntry logInfo)
        {
            var scopeProvider = ScopeProvider;

            if (_options.IncludeScopes && scopeProvider != null)
            {
                var scopeList = new List<string>();

                scopeProvider.ForEachScope((scope, list) =>
                {
                    list.Add(scope.ToString());
                }, (scopeList));

                logInfo.Scope = scopeList;
            }
        }
    }
}
