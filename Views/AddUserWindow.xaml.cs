using Library.Business.Managers;
using Library.Domain.Entities.Users;
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
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        AddUserWindowViewModel addUserWindowVm;

        public AddUserWindow(UserManager userManager)
        {
            InitializeComponent();
            addUserWindowVm = new(userManager);
            addUserWindowVm.EndWork += endWorkExecuted;
            this.Title = "Регистрация читателя";
            this.DataContext = addUserWindowVm;
            
        }

        public User? NewUser { get; private set; } = null;

        /// <summary>
        /// Обработчик события завершения работы.
        /// Закрывает окно.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void endWorkExecuted(object? sender, EventArgs e)
        {
            this.NewUser = addUserWindowVm.NewUser;
            this.DialogResult = addUserWindowVm.DialogResult;
            this.Close();
        }

    }
}
