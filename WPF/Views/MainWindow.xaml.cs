using bbHierarchicalGrid.ViewModels;
using System.Windows;
using WPF.Models;

namespace WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
         

        public MainWindow()
        {
            // create a new instance of view model
            // we are setting this instance to static property called ViewModel
            ViewModels.MainWindowViewModel.ViewModel = new bbViewModel<Item>(Properties.Settings.Default.LicenseKey);
            // Initialize view model (create columns)
            ViewModels.MainWindowViewModel.ViewModel.Initialize();
            // Initialize components of the main window
            InitializeComponent();
            // set data context of the bbHierarchicalGrid usercontrol 
            this.bbGrid.DataContext = ViewModels.MainWindowViewModel.ViewModel;
        }
    




    }

}
