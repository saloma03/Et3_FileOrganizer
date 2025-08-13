using System.Collections.Concurrent;
using FileOrganizer.Commands;
using FileOrganizer.Core.Interfaces;
using FileOrganizer.Core.Logging;
using FileOrganizer.Models;

namespace FileOrganizer.Core
{
    public class FilesOrganizer
    {
        #region fields
        private FileManager fileManager;
        private UndoManager undoManager;
        private ActionLogger actionLogger;
        private Dictionary<string, int> categoryCounts = new Dictionary<string, int>();
        private readonly List<Rule> rules;
        private readonly object _lock = new object();
        #endregion

        #region Constructors
        public FilesOrganizer(FileManager fileManager, SettingManager settingManager, UndoManager undoManager)
        {
            this.fileManager = fileManager;
            this.undoManager = undoManager;
            this.actionLogger = new ActionLogger();
            this.rules = settingManager.LoadRules(); 
        }

        #endregion

        #region Organization
        public void StartOrganization(string folderPath, bool simulate = false)
        {
            categoryCounts.Clear();
            var files = fileManager.ScanFolder(folderPath);

            var commands = new ConcurrentBag<ICommand>();
            Parallel.ForEach(files, file =>
            {
                string destinationFolder = DetermineDestination(file);

                lock (_lock)
                {
                    if (categoryCounts.ContainsKey(destinationFolder))
                    {
                        categoryCounts[destinationFolder]++;
                    }
                    else
                    {
                        categoryCounts[destinationFolder] = 1;
                    }
                }

                if (simulate)
                {
                    lock (_lock)
                    {
                        actionLogger.Log($"SIMULATE: Would move {file.Name} to {destinationFolder}");
                    }
                }
                else
                {
                    ICommand command = new OrganizeCommand(file, destinationFolder, fileManager);
                    commands.Add(command); 
                    lock (_lock)
                    {
                        actionLogger.Log($"Moved {file.Name} to {destinationFolder}");
                    }
                }
            });

            if (!simulate)
            {
                foreach (var command in commands)
                {
                    undoManager.Execute(command);
                }
            }

            LogCategorySummary();
        }
        private string DetermineDestination(FileModel file)
        {
            var matchingRule = rules.FirstOrDefault(rule => rule.Extension.Equals(file.Extension, StringComparison.OrdinalIgnoreCase));

            return matchingRule != null ? matchingRule.FolderName : "Others";
        }


        #endregion

        #region Undo 
        public void UndoLastOperation()
        {
            undoManager.Undo();
        }


        #endregion


        #region Final Move Summary
        private void LogCategorySummary()
        {
            actionLogger.Log("\n=== Organization Summary ===");
            foreach (var kvp in categoryCounts)
            {
                actionLogger.Log($"{kvp.Key}: {kvp.Value} files");
            }
            actionLogger.Log("==========================\n");
        }
        #endregion
    }
}
