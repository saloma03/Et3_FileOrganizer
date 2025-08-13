using System.IO.Abstractions.TestingHelpers;
using FileOrganizer.Core;
using FluentAssertions;

namespace FileOrganizer.Tests.IntegrationTest
{
    public class FilesOrganizerIntegrationTests
    {
        #region fields
        private readonly MockFileSystem _fileSystem;
        private readonly FilesOrganizer _organizer;
        private readonly string _testFolderPath = Path.Combine("C:", "test_folder");

        #endregion

        #region Constructors
        public FilesOrganizerIntegrationTests()
        {
            _fileSystem = new MockFileSystem();

            var fileManager = new FileManager(_fileSystem);
            var settingManager = new SettingManager(_fileSystem);
            var undoManager = new UndoManager();

            _organizer = new FilesOrganizer(fileManager, settingManager, undoManager);
            InitializeTestFolder();
        }

        #endregion

        private void InitializeTestFolder()
        {
            _fileSystem.AddDirectory(_testFolderPath);
            _fileSystem.AddFile(Path.Combine(_testFolderPath, "doc1.pdf"), new MockFileData("PDF content"));
            _fileSystem.AddFile(Path.Combine(_testFolderPath, "img1.jpg"), new MockFileData("JPG content"));
            _fileSystem.AddFile(Path.Combine(_testFolderPath, "vid1.mp4"), new MockFileData("MP4 content"));
            _fileSystem.AddFile(Path.Combine(_testFolderPath, "unknown.xyz"), new MockFileData("Unknown content"));
        }

        [Fact]
        public void StartOrganization_ShouldMoveFilesToCorrectFolders()
        {
            // Act
            _organizer.StartOrganization(_testFolderPath);

            // Assert
            _fileSystem.File.Exists(Path.Combine(_testFolderPath, "Documents", "doc1.pdf")).Should().BeTrue();
            _fileSystem.File.Exists(Path.Combine(_testFolderPath, "Images", "img1.jpg")).Should().BeTrue();
            _fileSystem.File.Exists(Path.Combine(_testFolderPath, "Videos", "vid1.mp4")).Should().BeTrue();
            _fileSystem.File.Exists(Path.Combine(_testFolderPath, "Others", "unknown.xyz")).Should().BeTrue();
        }

        [Fact]
        public void StartOrganization_InSimulationMode_ShouldNotMoveFiles()
        {
            // Act
            _organizer.StartOrganization(_testFolderPath, simulate: true);

            // Assert
            _fileSystem.File.Exists(Path.Combine(_testFolderPath, "doc1.pdf")).Should().BeTrue();
            _fileSystem.File.Exists(Path.Combine(_testFolderPath, "img1.jpg")).Should().BeTrue();
            _fileSystem.File.Exists(Path.Combine(_testFolderPath, "vid1.mp4")).Should().BeTrue();
            _fileSystem.File.Exists(Path.Combine(_testFolderPath, "unknown.xyz")).Should().BeTrue();
        }
        [Fact]
        public void UndoLastOperation_ShouldRevertFileMoves()
        {
            // Arrange
            var originalPdfPath = Path.Combine(_testFolderPath, "doc1.pdf");
            var originalJpgPath = Path.Combine(_testFolderPath, "img1.jpg");
            var originalMp4Path = Path.Combine(_testFolderPath, "vid1.mp4");
            var originalUnknownPath = Path.Combine(_testFolderPath, "unknown.xyz");

            _organizer.StartOrganization(_testFolderPath);

            _fileSystem.File.Exists(Path.Combine(_testFolderPath, "Documents", "doc1.pdf")).Should().BeTrue();
            _fileSystem.File.Exists(Path.Combine(_testFolderPath, "Images", "img1.jpg")).Should().BeTrue();
            _fileSystem.File.Exists(Path.Combine(_testFolderPath, "Videos", "vid1.mp4")).Should().BeTrue();
            _fileSystem.File.Exists(Path.Combine(_testFolderPath, "Others", "unknown.xyz")).Should().BeTrue();

            _organizer.UndoLastOperation(); // Undo unknown.xyz
            _organizer.UndoLastOperation(); // Undo vid1.mp4
            _organizer.UndoLastOperation(); // Undo img1.jpg
            _organizer.UndoLastOperation(); // Undo doc1.pdf

            _fileSystem.File.Exists(originalPdfPath).Should().BeTrue($"File doc1.pdf should exist at {originalPdfPath}");
            _fileSystem.File.Exists(originalJpgPath).Should().BeTrue($"File img1.jpg should exist at {originalJpgPath}");
            _fileSystem.File.Exists(originalMp4Path).Should().BeTrue($"File vid1.mp4 should exist at {originalMp4Path}");
            _fileSystem.File.Exists(originalUnknownPath).Should().BeTrue($"File unknown.xyz should exist at {originalUnknownPath}");

            _fileSystem.Directory.GetFiles(Path.Combine(_testFolderPath, "Documents")).Should().BeEmpty();
            _fileSystem.Directory.GetFiles(Path.Combine(_testFolderPath, "Images")).Should().BeEmpty();
            _fileSystem.Directory.GetFiles(Path.Combine(_testFolderPath, "Videos")).Should().BeEmpty();
            _fileSystem.Directory.GetFiles(Path.Combine(_testFolderPath, "Others")).Should().BeEmpty();
        }
    }

}
