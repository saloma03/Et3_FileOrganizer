using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileOrganizer.Core;
using FluentAssertions;

namespace FileOrganizer.Tests.UnitTests
{
    public class SettingManagerTests
    {

        #region Load Rules Test

        [Fact]
        public void LoadRules_WhenFileNotExists_ShouldReturnDefaultRules()

        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.Delete(tempFile); 
            var manager = new SettingManager(tempFile);

            // Act
            var rules = manager.LoadRules();

            // Assert
            rules.Should().NotBeEmpty();
            rules.Should().Contain(r => r.Extension == ".jpg" && r.FolderName == "Images");
            File.Exists(tempFile).Should().BeTrue(); 
        }
        #endregion

        #region Save Rules Test
        [Fact]
        public void SaveRules_ShouldCreateValidJsonFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.Delete(tempFile);
            var manager = new SettingManager(tempFile);
            var rules = new List<Rule>
            {
                new Rule { Extension = ".test", FolderName = "TestFolder" }
            };

            // Act
            manager.SaveRules(rules);
            var loadedRules = manager.LoadRules();

            // Assert
            loadedRules.Should().BeEquivalentTo(rules);
        } 
        #endregion
    }

}
