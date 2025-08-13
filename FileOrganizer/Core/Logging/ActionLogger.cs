using System.Collections.ObjectModel;

namespace FileOrganizer.Core.Logging
{
    internal class ActionLogger
    {
        #region Properties
        public static ObservableCollection<string> Logs { get; private set; } = new ObservableCollection<string>();

        #endregion

        #region Log
        public void Log(string message)
        {
            Logs.Add(message);
            System.Diagnostics.Debug.WriteLine(message);
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
