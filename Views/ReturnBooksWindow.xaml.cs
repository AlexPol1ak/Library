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
    /// Окно возврата книги.
    /// </summary>
    public partial class ReturnBooksWindow : Window
    {
        public ReturnBooksWindow(BookHistoryManager bookHistory, RackManager rackManager)
        {
            InitializeComponent();
            returnBooksVm = new ReturnBooksViewModel(bookHistory, rackManager);
            returnBooksVm.Title = "Вернуть книгу";
            returnBooksVm.EndWork += endWorkExecuted;
            this.DataContext = returnBooksVm;
        }

        private ReturnBooksViewModel returnBooksVm;

        /// <summary>
        /// Обработчик команды ViewModel завершения работы.
        /// Завершает работу окна.
        /// </summary>
        private void endWorkExecuted(object? sender, EventArgs e)
        {
            DialogResult = returnBooksVm.DialogResult;
            this.Close();
        }
      
    }
}
