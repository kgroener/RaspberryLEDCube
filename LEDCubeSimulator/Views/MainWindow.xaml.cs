using LEDCube.Simulator.WPF.ViewModels;
using System.IO;
using System.Text;
using System.Windows;

namespace LEDCube.Simulator.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
