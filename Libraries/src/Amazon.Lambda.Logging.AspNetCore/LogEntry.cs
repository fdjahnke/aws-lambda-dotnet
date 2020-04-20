using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Amazon.Lambda.Logging.AspNetCore
{
    public class LogEntry
    {
        public string LogLevel { get; set; }
        public string Category { get; set; }
        public int EventId { get; set; }
        public Exception Exception { get; set; }
        public string Text { get; set; }

        public List<string> Scope { get; set; }
    }
}
