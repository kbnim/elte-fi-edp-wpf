using System.DirectoryServices.ActiveDirectory;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RaceBike.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MenuWindow Menu { get; private set; } = new MenuWindow();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
            Menu.Closed += (sender, e) => Close();
        }

        private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            Menu.Show();
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            Menu.Close();
        }
    }
}