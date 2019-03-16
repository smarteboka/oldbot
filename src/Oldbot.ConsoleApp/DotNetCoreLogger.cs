using System;
using System.Collections.Generic;
using Common.Logging.Simple;
using Microsoft.Extensions.Logging;
using LogLevel = Common.Logging.LogLevel;

namespace Oldbot.ConsoleApp
{
    public class DotNetCoreLogger : AbstractSimpleLogger
    {
        private readonly ILogger<DotNetCoreLogger> _logger;
        
        private static readonly Dictionary<LogLevel, Microsoft.Extensions.Logging.LogLevel> Levels = new Dictionary<LogLevel, Microsoft.Extensions.Logging.LogLevel>
        {
            { LogLevel.Fatal, Microsoft.Extensions.Logging.LogLevel.Critical },
            { LogLevel.Error, Microsoft.Extensions.Logging.LogLevel.Error },
            { LogLevel.Warn, Microsoft.Extensions.Logging.LogLevel.Warning },
            { LogLevel.Info, Microsoft.Extensions.Logging.LogLevel.Information },
            { LogLevel.Debug, Microsoft.Extensions.Logging.LogLevel.Debug },
            { LogLevel.Trace, Microsoft.Extensions.Logging.LogLevel.Trace }
        };

        public DotNetCoreLogger(ILogger<DotNetCoreLogger> logger) : this("DotNetCoreLogger", LogLevel.All, true, true, true, "yyyy-MM-dd HH:mm:ss")
        {
            _logger = logger;
        }
        
        public DotNetCoreLogger(string logName, LogLevel logLevel, bool showlevel, bool showDateTime, bool showLogName, string dateTimeFormat) : base(logName, logLevel, showlevel, showDateTime, showLogName, dateTimeFormat)
        {
        }

        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {
            _logger.Log(Levels[level], message.ToString());
        }
    }
}