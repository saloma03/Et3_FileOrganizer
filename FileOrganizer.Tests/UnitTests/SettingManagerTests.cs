using System.IO.Abstractions.TestingHelpers;
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
            var fileSystem = new MockFileSystem();
            var tempFile = @"C:\temp\settings.json";
            var manager = new SettingManager(fileSystem, tempFile);
            var rules = manager.LoadRules();
            rules.Should().NotBeEmpty();
            rules.Should().Contain(r => r.Extension == ".jpg" && r.FolderName == "Images");
            fileSystem.File.Exists(tempFile).Should().BeTrue();
        }
        #endregion

        #region Save Rules Test
        [Fact]
        public void SaveRules_ShouldCreateValidJsonFile()
        {
            var fileSystem = new MockFileSystem();
            var tempFile = @"C:\temp\settings.json";
            var manager = new SettingManager(fileSystem, tempFile);
            var rules = new List<Rule>
    {
        new Rule { Extension = ".test", FolderName = "TestFolder" }
    };
            manager.SaveRules(rules);
            var loadedRules = manager.LoadRules();
            loadedRules.Should().BeEquivalentTo(rules);
            fileSystem.File.Exists(tempFile).Should().BeTrue();
        }
        #endregion
    }

}
