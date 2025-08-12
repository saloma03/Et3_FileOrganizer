using FileOrganizer.Commands;
using FileOrganizer.Core.Interfaces;
using FileOrganizer.Models;

namespace FileOrganizer.Core
{
    internal class FilesOrganizer
    {
        #region fields
        private FileManager fileManager;
        private UndoManager undoManager;
        private Dictionary<string, int> categoryCounts = new Dictionary<string, int>();
        private readonly List<Rule> rules;
        #endregion

        #region Constructors
        public FilesOrganizer(FileManager fileManager, SettingManager settingManager, UndoManager undoManager)
        {
            this.fileManager = fileManager;
            this.undoManager = undoManager;
            this.rules = settingManager.LoadRules(); 
        }

        #endregion

        #region Organization
        public void StartOrganization(string folderPath)
        {
            categoryCounts.Clear();
            var files = fileManager.ScanFolder(folderPath);
            foreach (var file in files)
            {
                string destinationFolder = DetermineDestination(file);

                // Update category count
                if (categoryCounts.ContainsKey(destinationFolder))
                {
                    categoryCounts[destinationFolder]++;
                }
                else
                {
                    categoryCounts[destinationFolder] = 1;
                }


                ICommand command = new OrganizeCommand(file, destinationFolder, fileManager);
                undoManager.Execute(command);

            }
        }

        private string DetermineDestination(FileModel file)
        {
            var matchingRule = rules.FirstOrDefault(rule => rule.Extension.Equals(file.Extension, StringComparison.OrdinalIgnoreCase));

            return matchingRule != null ? matchingRule.FolderName : "Others";
        }


        #endregion
    }
}
