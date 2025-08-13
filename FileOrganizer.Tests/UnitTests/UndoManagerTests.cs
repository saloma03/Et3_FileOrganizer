using FileOrganizer.Core;
using FileOrganizer.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace FileOrganizer.Tests.UnitTests
{
    public class UndoManagerTests
    {
        #region Execute test
        [Fact]
        public void Execute_ShouldCallCommandAndAddToHistory()
        {
            // Arrange
            var mockCommand = new Mock<ICommand>();
            var undoManager = new UndoManager();

            // Act
            undoManager.Execute(mockCommand.Object);

            // Assert
            mockCommand.Verify(c => c.Execute(), Times.Once);
            undoManager.getHistoryCounts().Should().Be(1);
        }
        #endregion

        #region undo test

        [Fact]
        public void Undo_WithEmptyHistory_ShouldDoNothing()
        {
            // Arrange
            var undoManager = new UndoManager();

            // Act & Assert
            undoManager.Invoking(u => u.Undo())
                .Should().NotThrow();
        } 
        #endregion
    }

}
