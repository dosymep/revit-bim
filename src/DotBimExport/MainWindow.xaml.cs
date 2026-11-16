using System.Windows;

using DotBimExport.ViewModels;

namespace DotBimExport;

/// <summary>
/// Export settings dialog for a dotbim file.
/// </summary>
public partial class MainWindow : Window {
    /// <summary>
    /// Initializes the dialog bound to <paramref name="viewModel"/>.
    /// </summary>
    /// <param name="viewModel">Export settings.</param>
    public MainWindow(MainViewModel viewModel) {
        DataContext = viewModel;
        InitializeComponent();

        viewModel.CloseRequested += (_, _) => DialogResult = true;
        viewModel.AlertRequested += (_, message) => MessageBox.Show(
            this,
            message,
            Title,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}
