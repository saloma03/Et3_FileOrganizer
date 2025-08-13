using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Windows;
using System.Windows.Forms;
using FileOrganizer.Core;
using FileOrganizer.Core.Logging;

namespace FileOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region fields
        private FilesOrganizer fileOrganizer;

        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
            IFileSystem realFileSystem = new FileSystem();

            var realFileManager = new FileManager(realFileSystem);
            var realSettingManager = new SettingManager(realFileSystem);
            var realUndoManager = new UndoManager(realFileSystem);
            fileOrganizer = new FilesOrganizer(realFileManager, realSettingManager, realFileSystem);
        }

        #endregion

        #region Organize 
        private void OrganizeButton_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = FolderPathTextBox.Text.Trim();

            if (string.IsNullOrEmpty(folderPath))
            {
                StatusText.Text = "Please select a folder to organize.";
                return;
            }

            if (!System.IO.Directory.Exists(folderPath))
            {
                StatusText.Text = "The selected folder does not exist.";
                return;
            }

            try
            {
                ActionLogger.ClearLogs();

                fileOrganizer.StartOrganization(folderPath, simulate: true);

                ObservableCollection<string> simulationLogs = ActionLogger.Logs;

                // show preview 
                var previewWindow = new PreviewWindow(folderPath, simulationLogs, fileOrganizer);
                previewWindow.Owner = this;

                this.Hide();
                previewWindow.ShowDialog();
                this.Show();

                //StatusText.Text = previewWindow.UserConfirmed
                //    ? "Organization completed successfully!"
                //    : "Preview completed. No changes were made.";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
            }
        }

        #endregion

        #region Undo 
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = System.Windows.MessageBox.Show(
                    "Are you sure you want to undo the last operation?",
                    "Confirm Undo",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    fileOrganizer.UndoLastOperation();
                    StatusText.Text = "Last operation undone.";
                }
                else
                {
                    StatusText.Text = "Undo canceled.";
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Undo failed: {ex.Message}";
            }
        }

        private void UndoAllButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string folderPath = FolderPathTextBox.Text.Trim();
                if (string.IsNullOrEmpty(folderPath) || !System.IO.Directory.Exists(folderPath))
                {
                    StatusText.Text = "Please select a valid folder first.";
                    return;
                }

                var result = System.Windows.MessageBox.Show(
                    "Are you sure you want to undo ALL operations and clean up empty folders?",
                    "Confirm Undo All",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    fileOrganizer.UndoAllAndCleanup(folderPath);
                    StatusText.Text = "All operations undone and empty folders cleaned up.";
                }
                else
                {
                    StatusText.Text = "Undo All canceled.";
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Undo all failed: {ex.Message}. Check logs for details.";
            }
        }

        #endregion

        #region Browse
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select Folder to Organize";
                    folderDialog.ShowNewFolderButton = true;

                    if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        FolderPathTextBox.Text = folderDialog.SelectedPath;
                        StatusText.Text = $"Selected folder: {folderDialog.SelectedPath}";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error selecting folder: {ex.Message}";
            }
        }
        #endregion
    }
}