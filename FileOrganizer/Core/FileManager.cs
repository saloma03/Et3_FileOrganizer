using System.IO;
using System.IO.Abstractions;
using FileOrganizer.Models;

namespace FileOrganizer.Core

{
    public class FileManager
    {
        #region properties
        private readonly IFileSystem _fileSystem;
        #endregion

        #region constructors
        public FileManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        #endregion

        #region Getters
        public IFileSystem GetFileSystem()
        {
            return _fileSystem;
        }

        #endregion

        #region Scan Folder

        /*this function iterats in the folder and stores every file 
        [name, path, extension, originalpath] in list and return it 
        */
        public List<FileModel> ScanFolder(string path)
        {
            var files = new List<FileModel>();

            foreach (var filePath in _fileSystem.Directory.GetFiles(path))
            {
                files.Add(new FileModel
                {
                    Name = _fileSystem.Path.GetFileName(filePath),
                    Path = filePath,
                    Extension = _fileSystem.Path.GetExtension(filePath),
                    OriginalPath = filePath
                });
            }
            return files;
        }


        #endregion

        #region Move File [for organizing]

        ///this function create the new path for the file if the directory is not exists 
        ///it creats it and move the file to it else it move the file to it
        ///first it gets the directory name and then combine it with the new organize directory 
        ///then it will combine it with file name 
        public void MoveFile(FileModel file, string destinationFolder)
        {
            string destDirectory = _fileSystem.Path.Combine(_fileSystem.Path.GetDirectoryName(file.Path), destinationFolder);
            string destPath = _fileSystem.Path.Combine(destDirectory, file.Name);

            if (!_fileSystem.Directory.Exists(destDirectory))
            {
                _fileSystem.Directory.CreateDirectory(destDirectory);
            }

            if (_fileSystem.File.Exists(destPath))
            {
                HandleFileConflict(ref destPath);
            }
            if (string.IsNullOrEmpty(file.OriginalPath))
            {
                file.OriginalPath = file.Path;
            }

            _fileSystem.File.Move(file.Path, destPath);
            file.Path = destPath;
        }
        private void HandleFileConflict(ref string destinationPath)
        {
            // Option 1: Skip (just return without moving)
            // return;

            // Option 2: Overwrite (delete existing)
            // File.Delete(destinationPath);

            // Option 3: Rename 
            string dir = Path.GetDirectoryName(destinationPath);
            string fileName = Path.GetFileNameWithoutExtension(destinationPath);
            string ext = Path.GetExtension(destinationPath);
            destinationPath = Path.Combine(dir, $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{ext}");
        }
        #endregion

        #region Move File to Original Path [for undo]

        ///first if the directory is not exists [I think it is  impossible but for more secure code :>]
        ///it will create it and move the folder to it 
        ///if the file is exists dont create it handle the conflict and move the folder
        ///if the folder created in organization is empty remove it 
        public void MoveFileToFullPath(FileModel file, string fullPath)
        {
            try
            {
                var originalDir = Path.GetDirectoryName(fullPath);
                if (!_fileSystem.Directory.Exists(originalDir))
                {
                    _fileSystem.Directory.CreateDirectory(originalDir);
                }

                if (_fileSystem.File.Exists(fullPath))
                {
                    HandleFileConflict(ref fullPath);
                }

                _fileSystem.File.Move(file.Path, fullPath);
                file.Path = fullPath;

                var sourceDir = Path.GetDirectoryName(file.Path);
                if (_fileSystem.Directory.Exists(sourceDir) &&
                    !_fileSystem.Directory.GetFiles(sourceDir).Any() &&
                    !_fileSystem.Directory.GetDirectories(sourceDir).Any())
                {
                    _fileSystem.Directory.Delete(sourceDir);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while moving file: {ex.Message}");
                throw;
            }
        }

        #endregion

    }
}
