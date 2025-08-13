using System.Diagnostics;
using System.Windows;
using FileOrganizer.Core.Interfaces;
using System.IO.Abstractions;

namespace FileOrganizer.Core
{
    public class UndoManager
    {
        #region Fields
        private readonly Stack<ICommand> _history = new Stack<ICommand>();
        private readonly IFileSystem _fileSystem;
        #endregion

        #region Constructor
        public UndoManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }
        #endregion

        #region Getters
        public int GetHistoryCount()
        {
            return _history.Count;
        }
        #endregion

        #region Execute
        public void Execute(ICommand command)
        {
            command.Execute();
            _history.Push(command);
            Trace.WriteLine($"[UndoManager] Command added. Stack size: {_history.Count}");
        }
        #endregion

        #region Undo Operations
        public void Undo()
        {
            if (_history.Count > 0)
            {
                ICommand command = _history.Pop();
                command.Undo();
            }
            else
            {
                MessageBox.Show("No actions to undo.");
            }
        }

        public void UndoAllAndCleanup(FileManager fileManager, string originalFolder)
        {
            // Undo all commands
            while (_history.Count > 0)
            {
                Undo();
            }

            // Cleanup target folders
            string[] targetFolders = { "Documents", "Images", "Others", "Videos", "Designs" };
            foreach (var folder in targetFolders)
            {
                CleanupFolder(originalFolder, folder);
            }

            // Recursively cleanup empty directories
            DeleteEmptyDirectoriesRecursively(fileManager, originalFolder);
        }

        private void CleanupFolder(string basePath, string folderName)
        {
            try
            {
                string fullFolderPath = _fileSystem.Path.Combine(basePath, folderName);

                if (!_fileSystem.Directory.Exists(fullFolderPath))
                    return;

                var files = _fileSystem.Directory.GetFiles(fullFolderPath);
                var subDirs = _fileSystem.Directory.GetDirectories(fullFolderPath);

                if (!files.Any() && !subDirs.Any())
                {
                    _fileSystem.Directory.Delete(fullFolderPath, recursive: false);
                    Trace.WriteLine($"Deleted empty folder: {fullFolderPath}");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.WriteLine($"Access denied while deleting {folderName}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error deleting {folderName}: {ex.Message}");
            }
        }

        private void DeleteEmptyDirectoriesRecursively(FileManager fileManager, string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || !fileManager.GetFileSystem().Directory.Exists(folderPath))
                return;

            try
            {
                // First process all subdirectories
                foreach (var dir in fileManager.GetFileSystem().Directory.GetDirectories(folderPath))
                {
                    DeleteEmptyDirectoriesRecursively(fileManager, dir);
                }

                // Then check if current directory is empty
                var files = fileManager.GetFileSystem().Directory.GetFiles(folderPath);
                var dirs = fileManager.GetFileSystem().Directory.GetDirectories(folderPath);

                if (!files.Any() && !dirs.Any())
                {
                    fileManager.GetFileSystem().Directory.Delete(folderPath, recursive: false);
                    Trace.WriteLine($"Deleted empty directory: {folderPath}");

                    // Check parent directory
                    var parent = fileManager.GetFileSystem().Path.GetDirectoryName(folderPath);
                    if (!string.IsNullOrEmpty(parent))
                    {
                        DeleteEmptyDirectoriesRecursively(fileManager, parent);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error processing directory {folderPath}: {ex.Message}");
            }
        }
        #endregion
    }
}