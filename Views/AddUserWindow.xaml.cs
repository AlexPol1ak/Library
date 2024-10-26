using Library.Business.Managers;
using Library.Domain.Entities.Users;
using Library.ViewModels;
using System.Windows;

namespace Library.Views
{
    /// <summary>
    /// Окно добавления читателя.
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
