using System;
using Fanex.Module.Logs.Services;
using Sunplus.Accounting.Core.Extensions;

namespace Fanex.BetList.Accounting.Core
{
    public enum LogType : int
    {
        Info = 1,
        Error = 2
    }

    public class LoggingHandler
    {
        public static void Log(Exception ex)
        {
            ex.WriteLog();
        }

        public static void Log(string message = "", LogType logType = LogType.Error)
        {
            var ex = new Exception(message);
            ex.Write();
        }
    }
}