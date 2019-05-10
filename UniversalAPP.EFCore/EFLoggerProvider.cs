using Microsoft.Extensions.Logging;
using System;

namespace UniversalAPP.EFCore
{
    public class EFLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new EFLogger(categoryName);

        public void Dispose() { }
    }

    public class EFLogger : ILogger
    {
        private readonly string categoryName;

        public EFLogger(string categoryName) => this.categoryName = categoryName;

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //ef core执行数据库查询时的categoryName为Microsoft.EntityFrameworkCore.Database.Command,日志级别为Information
            if (categoryName == "Microsoft.EntityFrameworkCore.Database.Command" && logLevel == LogLevel.Information)
            {
                //Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                var logContent = formatter(state, exception);
                System.Diagnostics.Trace.WriteLine(logContent);
                //拿到日志内容想怎么玩就怎么玩吧
                //Console.WriteLine();
                //Console.WriteLine(logContent);
                //Console.WriteLine();
            }
        }
    }
}
