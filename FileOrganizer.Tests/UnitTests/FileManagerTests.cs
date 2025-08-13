using FileOrganizer.Core;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using FileOrganizer.Models;

namespace FileOrganizer.Tests.UnitTests
{
    public class FileManagerTests
    {
        #region fields
        private readonly MockFileSystem _fileSystem;
        private readonly FileManager _fileManager;
        #endregion

        #region constructor
        public FileManagerTests()
        {
            _fileSystem = new MockFileSystem();
            _fileManager = new FileManager(_fileSystem);
        }

        #endregion


        #region MoveFile Tests
        [Fact]
        public void MoveFile_ShouldUpdateFileProperties()
        {
            // Arrange
            var sourceDir = Path.Combine("C:", "source");
            var sourceFile = Path.Combine(sourceDir, "file.txt");
            var destFolder = @"C:\destination";

            _fileSystem.AddDirectory(sourceDir);
            _fileSystem.AddFile(sourceFile, new MockFileData("content"));
            _fileSystem.AddDirectory(destFolder);

            var file = new FileModel
            {
                Name = "file.txt",
                Path = sourceFile,
                Extension = ".txt"
            };

            // Act
            _fileManager.MoveFile(file, destFolder);

            // Assert
            file.Path.Should().Be(@"C:\destination\file.txt");
            file.OriginalPath.Should().Be(sourceFile);
            _fileSystem.File.Exists(@"C:\destination\file.txt").Should().BeTrue();
            _fileSystem.File.Exists(sourceFile).Should().BeFalse();
        }
        #endregion

        #region ScanFolder Tests
        [Fact]
        public void ScanFolder_ShouldReturnAllFiles()
        {
            // Arrange
            var testDir = Path.Combine("C:", "test");
            var file1 = Path.Combine(testDir, "file1.txt");
            var file2 = Path.Combine(testDir, "file2.jpg");

            _fileSystem.AddDirectory(testDir);
            _fileSystem.AddFile(file1, new MockFileData("test content"));
            _fileSystem.AddFile(file2, new MockFileData("test content"));

            // Act
            var result = _fileManager.ScanFolder(testDir);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(f => f.Name == "file1.txt");
            result.Should().Contain(f => f.Name == "file2.jpg");
        }
        #endregion

    }

}
