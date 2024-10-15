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
    /// Окно добавления заявки.
    /// </summary>
    public partial class AddRequestWindow : Window
    {
        AddRequestWindowViewModel addRequestWindowVm;
        public Request? NewRequest { get; private set; } = null;

        public AddRequestWindow(RequestManager requestManager, UserManager userManager,
            BookManager bookManager, BookHistoryManager bookHistoryManager)
        {
            InitializeComponent();

            addRequestWindowVm = new AddRequestWindowViewModel(requestManager, userManager, 
                bookManager, bookHistoryManager);
            addRequestWindowVm.Title = "Добавить запрос";
            addRequestWindowVm.EndWork += EndWork_Executed;
            this.DataContext = addRequestWindowVm;
            
        }

        /// <summary>
        /// Обработчик команды ViewModel завершения работы.
        /// Завершает работу окна.
        /// </summary>
        private void EndWork_Executed(object? sender, EventArgs e)
        {
            this.DialogResult = addRequestWindowVm.DialogResult;
            this.NewRequest = addRequestWindowVm.NewRequest;
            this.Close();
        }
    }
}
