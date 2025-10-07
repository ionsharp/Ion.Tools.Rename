using Microsoft.Win32;
using System.Windows;

namespace Ion.Tools.Rename;

/// <remarks>https://stackoverflow.com/questions/923771/quickest-way-to-convert-a-base-10-number-to-any-base-in-net</remarks>
public partial class MainView : Window
{
    private MainViewModel viewModel = new();

    public MainView()
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void onBrowse(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            Title = "Select folder",
        };

        if (dialog.ShowDialog() == true)
        {
            string selectedPath = dialog.FolderName;
            viewModel.FolderPath = selectedPath;
        }
    }

    private void onDo(object sender, RoutedEventArgs e)
    {
        viewModel.Do();
    }
}