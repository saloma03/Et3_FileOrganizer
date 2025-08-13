using System.Collections.ObjectModel;
using Serilog;

namespace FileOrganizer.Core.Logging
{
    internal class ActionLogger
    {
        #region properties
        private readonly ILogger _logger;

        #endregion
        #region Properties
        public static ObservableCollection<string> Logs { get; private set; } = new ObservableCollection<string>();

        #endregion
        #region Constructors
        public ActionLogger()
        {
            _logger = Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        #endregion
        #region Log
        public void LogMessage(string message)
        {
            Logs.Add(message);
            _logger.Information(message);
        }

        #endregion

        #region Clear Log
        public static void ClearLogs()
        {
            Logs.Clear();
        }

        #endregion
    }
}

