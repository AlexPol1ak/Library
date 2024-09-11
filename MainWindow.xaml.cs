using Library.Business.Infastructure;
using Library.Business.Infastructure.DbFakeData;
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
        public MainWindow()
        {
            InitializeComponent();
            ManagersFactory mf =  new ManagersFactory("DefaultConnection", "MySQLVersion");
            new FakeData(mf, "books_and_authors.json").InstallData();
        }
    }
}