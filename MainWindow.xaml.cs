using Library.Business.Infastructure;
using Library.Business.Infastructure.DbFakeData;
using Library.ViewModels;
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

namespace Library
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel mainWindowViewModel;//ViewModel главного окна 
        ManagersFactory managersFactory;

        public MainWindow()
        {
            InitializeComponent();
            managersFactory =  new ManagersFactory("DefaultConnection", "MySQLVersion");
            //if (new FakeData(managersFactory).InstallData() is bool flag) MessageBox.Show($"Установка начальных данных: {flag}");

            mainWindowViewModel = new (managersFactory);
            this.DataContext = mainWindowViewModel;
        }

    }
}