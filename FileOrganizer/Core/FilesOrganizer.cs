using System.Windows;
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
        #endregion

        #region Constructors
        public FilesOrganizer(FileManager fileManager, SettingManager settingManager, IFileSystem fileSystem)
        {
            this.fileManager = fileManager;
            this.undoManager = new UndoManager(fileSystem);
            this.actionLogger = new ActionLogger();
            this.rules = settingManager.LoadRules(); 
        }

        #endregion

        #region Organization
        public void StartOrganization(string folderPath, bool simulate = false)
        {
            categoryCounts.Clear();
            var files = fileManager.ScanFolder(folderPath);
            foreach (var file in files)
            {
                string destinationFolder = DetermineDestination(file);
                if (categoryCounts.ContainsKey(destinationFolder))
                {
                    categoryCounts[destinationFolder]++;
                }
                else
                {
                    categoryCounts[destinationFolder] = 1;
                }
                if (simulate)
                {
                    actionLogger.LogMessage($"SIMULATE: Would move {file.Name} to {destinationFolder}");
                }
                else
                {
                    ICommand command = new OrganizeCommand(file, destinationFolder, fileManager);
                    undoManager.Execute(command);
                    actionLogger.LogMessage($"Moved {file.Name} to {destinationFolder}");
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
        public void UndoAllAndCleanup(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || !System.IO.Directory.Exists(folderPath))
            {
                actionLogger.LogMessage("Error: Invalid or non-existent folder path for cleanup.");
                return;
            }

            try
            {
                undoManager.UndoAllAndCleanup(fileManager, folderPath);
                actionLogger.LogMessage($"All operations undone and empty folders in {folderPath} cleaned up.");
            }
            catch (Exception ex)
            {
                actionLogger.LogMessage($"Error during UndoAllAndCleanup: {ex.Message}");
                MessageBox.Show($"Failed to undo all operations: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion


        #region Final Move Summary
        private void LogCategorySummary()
        {
            actionLogger.LogMessage("\n=== Organization Summary ===");
            foreach (var kvp in categoryCounts)
            {
                actionLogger.LogMessage($"{kvp.Key}: {kvp.Value} files");
            }
            actionLogger.LogMessage("==========================\n");
        }
        #endregion
    }
}
