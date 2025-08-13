using System.IO.Abstractions;
using FileOrganizer.Core;
using FileOrganizer.Core.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FileOrganizer.Tests.UnitTests
{
    public class UndoManagerTests
    {
        private readonly Mock<IFileSystem> _mockFileSystem;
        private readonly Mock<FileManager> _mockFileManager;


        public UndoManagerTests()
        {
            _mockFileSystem = new Mock<IFileSystem>();
            _mockFileManager = new Mock<FileManager>(_mockFileSystem.Object);
        }        

        #region Execute test
        [Fact]
        public void Execute_ShouldCallCommandAndAddToHistory()
        {
            // Arrange
            var mockCommand = new Mock<ICommand>();
            var undoManager = new UndoManager(_mockFileSystem.Object);

            // Act
            undoManager.Execute(mockCommand.Object);

            // Assert
            mockCommand.Verify(c => c.Execute(), Times.Once);
            undoManager.GetHistoryCount().Should().Be(1);
        }
        #endregion

        #region undo test
        [Fact]
        public void Undo_WithEmptyHistory_ShouldDoNothing()
        {
            // Arrange
            var undoManager = new UndoManager(_mockFileSystem.Object);

            // Act & Assert
            undoManager.Invoking(u => u.Undo())
                .Should().NotThrow();
        }

        [Fact]
        public void Undo_WithHistory_ShouldCallUndoOnCommand()
        {
            // Arrange
            var mockCommand = new Mock<ICommand>();
            var undoManager = new UndoManager(_mockFileSystem.Object);
            undoManager.Execute(mockCommand.Object);

            // Act
            undoManager.Undo();

            // Assert
            mockCommand.Verify(c => c.Undo(), Times.Once);
        }
        #endregion

        #region UndoAllAndCleanup tests
        [Fact]
        public void UndoAllAndCleanup_WithEmptyHistory_ShouldNotThrow()
        {
            // Arrange
            _mockFileSystem.Setup(fs => fs.Directory.Exists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(fs => fs.Directory.GetFiles(It.IsAny<string>())).Returns(Array.Empty<string>());
            _mockFileSystem.Setup(fs => fs.Directory.GetDirectories(It.IsAny<string>())).Returns(Array.Empty<string>());

            var undoManager = new UndoManager(_mockFileSystem.Object);

            // Act & Assert
            undoManager.Invoking(u => u.UndoAllAndCleanup(_mockFileManager.Object, "testPath"))
                .Should().NotThrow();

            // Ensure that deletion is triggered for empty folder
            _mockFileSystem.Verify(fs => fs.Directory.Delete(It.IsAny<string>(), false), Times.Once());
        }

        [Fact]
        public void UndoAllAndCleanup_WithNonEmptyFolder_ShouldNotDelete()
        {
            // Arrange
            _mockFileSystem.Setup(fs => fs.Directory.Exists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(fs => fs.Directory.GetFiles(It.IsAny<string>())).Returns(new[] { "file1.txt" });
            _mockFileSystem.Setup(fs => fs.Directory.GetDirectories(It.IsAny<string>())).Returns(Array.Empty<string>());

            var undoManager = new UndoManager(_mockFileSystem.Object);

            // Act
            undoManager.UndoAllAndCleanup(_mockFileManager.Object, "testPath");

            // Assert
            _mockFileSystem.Verify(fs => fs.Directory.Delete(It.IsAny<string>(), false), Times.Never);
        }
        #endregion
    }
}