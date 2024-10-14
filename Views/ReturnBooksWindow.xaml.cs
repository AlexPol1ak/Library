using Library.Business.Managers;
using Library.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Library.Views
{
    /// <summary>
    /// Логика взаимодействия для ReturnBooksWindow.xaml
    /// </summary>
    public partial class ReturnBooksWindow : Window
    {
        public ReturnBooksWindow(BookHistoryManager bookHistory)
        {
            InitializeComponent();
            returnBooksVm = new ReturnBooksViewModel(bookHistory);
            returnBooksVm.Title = "Вернуть книгу";
            this.DataContext = returnBooksVm;
        }

        private ReturnBooksViewModel returnBooksVm;

    }
}
