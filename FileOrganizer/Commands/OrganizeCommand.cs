

using System.Diagnostics;
using FileOrganizer.Core;
using FileOrganizer.Core.Interfaces;
using FileOrganizer.Models;

namespace FileOrganizer.Commands
{
    internal class OrganizeCommand : ICommand
    {
        #region fields
        private FileModel file;
        private string destination;
        private FileManager fileManager;
        private readonly IFileSystem fileSystem;
        #endregion

        #region Constructor
        public OrganizeCommand(FileModel file, string destination, FileManager fileManager)
        {
            this.file = file ?? throw new ArgumentNullException(nameof(file));
            this.destination = destination ?? throw new ArgumentNullException(nameof(destination));
            this.fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
            if (string.IsNullOrEmpty(file.OriginalPath))
            {
                file.OriginalPath = file.Path;
            }
            this.fileSystem = fileManager.GetFileSystem();
        }

        #endregion

        #region Execute
        public void Execute()
        {
            try
            {
                fileManager.MoveFile(file, destination);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to execute move for {file.Name}: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Undo
        public void Undo()
        {
            if (string.IsNullOrEmpty(file.OriginalPath))
            {
                return;
            }

            try
            {
                var originalDir = fileSystem.Path.GetDirectoryName(file.OriginalPath);
                if (!fileSystem.Directory.Exists(originalDir))
                {
                    fileSystem.Directory.CreateDirectory(originalDir);
                }

                fileManager.MoveFileToFullPath(file, file.OriginalPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to undo move for {file.Name}: {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}
