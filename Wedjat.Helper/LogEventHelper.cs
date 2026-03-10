using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model.Config;
using Wedjat.Model.Entity;

namespace Wedjat.Helper
{
    public static class LogEventHelper
    {
        public static event EventHandler<LogEventArgs> Log;
      
        public static async Task PushLog(LogType type, string message, LogStatu status)
        {
            //Log?.Invoke(null, new LogEventArgs
            //{
            //    LogType = type,
            //    Message = message,
            //    Status = status
            //});
            var logHandler = Log;
            if (logHandler != null)
            {

                logHandler.Invoke(null, new LogEventArgs
                {
                    LogType = type,
                    Message = message,
                    Status = status
                });
            }

            await Task.CompletedTask;
        }

    }
}
