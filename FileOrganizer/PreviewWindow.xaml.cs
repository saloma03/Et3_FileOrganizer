using System.Collections.ObjectModel;
using System.Windows;
using FileOrganizer.Core;

namespace FileOrganizer
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        #region Fields

        private string _folderPath;
        private FilesOrganizer organizer;
        private ObservableCollection<string> _simulationLogs;

        #endregion

        #region Properties
        public bool UserConfirmed { get; private set; }

        #endregion

        #region Constructors
        public PreviewWindow(string folderPath, ObservableCollection<string> simulationLogs, FilesOrganizer organizer)
        {
            try
            {
                InitializeComponent();
                _folderPath = folderPath;
                _simulationLogs = simulationLogs;
                DisplaySimulationLogs();
                this.organizer = organizer; // Store the organizer instance

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing PreviewWindow: {ex.InnerException?.Message ?? ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        #endregion

        #region Simulation
        private void DisplaySimulationLogs()
        {
            simulationLogListBox.ItemsSource = _simulationLogs;
        }
        #endregion


        #region Handle User Decision
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            organizer.StartOrganization(_folderPath, false);

            MessageBox.Show("File Organization Done Successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            UserConfirmed = true;
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            UserConfirmed = false;
            this.Close();
        }

        #endregion

    }
}
