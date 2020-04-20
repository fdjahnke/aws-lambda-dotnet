using System;
using System.Collections.Generic;
using Amazon.Lambda.Logging.AspNetCore;

namespace Microsoft.Extensions.Logging
{
	internal class LambdaILogger : LambdaBaseLogger
	{
		// Constructor
		public LambdaILogger(string categoryName, LambdaLoggerOptions options): base(categoryName, options){ }

		public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (formatter == null)
			{
				throw new ArgumentNullException(nameof(formatter));
			}

			if (!IsEnabled(logLevel))
			{
				return;
			}

			// Format of the logged text, optional components are in {}
			//  {[LogLevel] }{ => Scopes : }{Category: }{EventId: }MessageText {Exception}{\n}

			var components = new List<string>(4);
			if (_options.IncludeLogLevel)
			{
				components.Add($"[{logLevel}]");
			}

			GetScopeInformation(components);

			if (_options.IncludeCategory)
			{
				components.Add($"{_categoryName}:");
			}
			if (_options.IncludeEventId)
			{
				components.Add($"[{eventId}]:");
			}

			var text = formatter.Invoke(state, exception);
			components.Add(text);

			if (_options.IncludeException)
			{
				components.Add($"{exception}");
			}
			if (_options.IncludeNewline)
			{
				components.Add(Environment.NewLine);
			}

			var finalText = string.Join(" ", components);
			Amazon.Lambda.Core.LambdaLogger.Log(finalText);
		}

		private void GetScopeInformation(List<string> logMessageComponents)
		{
			var scopeProvider = ScopeProvider;

			if (_options.IncludeScopes && scopeProvider != null)
			{
				var initialCount = logMessageComponents.Count;

				scopeProvider.ForEachScope((scope, list) =>
				{
					list.Add(scope.ToString());
				}, (logMessageComponents));

				if (logMessageComponents.Count > initialCount)
				{
					logMessageComponents.Add("=>");
				}
			}
		}
    }
}
