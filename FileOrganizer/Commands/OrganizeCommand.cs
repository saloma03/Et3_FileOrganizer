

using System.Diagnostics;
using System.IO.Abstractions;
using FileOrganizer.Core;
using FileOrganizer.Core.Interfaces;
using FileOrganizer.Models;

namespace FileOrganizer.Commands
{
    internal class OrganizeCommand : ICommand
    {
        #region fields
        private FileModel file;
        private string originalPath;
        private string destination;
        private FileManager fileManager;
        private readonly IFileSystem fileSystem;
        #endregion

        #region Constructor
        public OrganizeCommand(FileModel file, string destination, FileManager fileManager)
        {
            if (string.IsNullOrEmpty(file.OriginalPath))
            {
                file.OriginalPath = file.Path;
            }
            this.file = file;
            this.destination = destination;
            this.fileManager = fileManager;
            this.fileSystem = fileManager.GetFileSystem(); // يجب إضافة هذه الدالة في FileManager

        }
        #endregion

        #region Execute
        public void Execute()
        {
            fileManager.MoveFile(file, destination);
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
