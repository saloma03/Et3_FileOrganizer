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
        public List<FileModel> ScanFolder(string path, bool recursive = false)
        {
            var files = new List<FileModel>();
            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filePath in _fileSystem.Directory.GetFiles(path, "*", searchOption))
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
            string dir = _fileSystem.Path.GetDirectoryName(destinationPath);
            string fileName = _fileSystem.Path.GetFileNameWithoutExtension(destinationPath);
            string ext = _fileSystem.Path.GetExtension(destinationPath);
            destinationPath = _fileSystem.Path.Combine(dir, $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{ext}");
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
                var originalDir = _fileSystem.Path.GetDirectoryName(fullPath);
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

                var sourceDir = _fileSystem.Path.GetDirectoryName(file.Path);
                DeleteEmptyDirectory(sourceDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while moving file: {ex.Message}");
                throw;
            }
        }

        private void DeleteEmptyDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath) || !_fileSystem.Directory.Exists(directoryPath))
                return;

            try
            {
                if (!_fileSystem.Directory.GetFiles(directoryPath).Any() &&
                    !_fileSystem.Directory.GetDirectories(directoryPath).Any())
                {
                    _fileSystem.Directory.Delete(directoryPath, recursive: false);
                    var parentDirectory = _fileSystem.Path.GetDirectoryName(directoryPath);
                    if (!string.IsNullOrEmpty(parentDirectory))
                    {
                        DeleteEmptyDirectory(parentDirectory);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied while deleting directory {directoryPath}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting directory {directoryPath}: {ex.Message}");
            }
        }
        #endregion

    }
}
