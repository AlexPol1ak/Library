using Library.Business.Infastructure;
using Library.Business.Infastructure.DbFakeData;
using Library.Domain.Entities.Users;
using Library.ViewModels;
using Library.Views;
using System.ComponentModel;
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
    /// Главное окно.
    /// </summary>
    public partial class MainWindow : Window
    {
        //ViewModel главного окна 
        MainWindowViewModel mainWindowViewModel;  
        // Фабрика менеджеров
        ManagersFactory managersFactory;

        public MainWindow()
        {
            InitializeComponent();
            managersFactory = new ManagersFactory("DefaultConnection", "MySQLVersion");
            this.Title = "Библиотека. ";

            StartLoadingAndAuthentication();
        }

        /// <summary>
        /// Инициализирует загрузку данных в ViewModel параллельно с аутентификацией.
        /// Если аутентификация успешна, продолжает инициализацию и устанавливает DataContext.
        /// Если аутентификация неудачна, закрывает приложение.
        /// </summary>
        private async void StartLoadingAndAuthentication()
        {
            // Запуск задачи инициализации ViewModel и загрузки данных
            var initializeViewModelTask = InitializeViewModelAsync();

            // Параллельно отображаем окно аутентификации
            //bool authSuccess = await auntification();
          
            //if (!authSuccess)
            //{
            //    this.Close(); 
            //    return;
            //}
            
            //await initializeViewModelTask;           
        }

        /// <summary>
        /// Метод для инициализации ViewModel и установки DataContext
        /// </summary>
        private async Task InitializeViewModelAsync()
        {
            await Task.Delay(10);
            //if (new FakeData(managersFactory).InstallData() is bool flag) MessageBox.Show($"Установка начальных данных: {flag}");
            mainWindowViewModel = new MainWindowViewModel(managersFactory);

            this.Dispatcher.Invoke(() =>
            {
                this.DataContext = mainWindowViewModel;
            });
        }

        /// <summary>
        /// Метод для отображения окна аутентификации
        /// </summary>
        async private Task<bool> auntification()
        {            
            AuntificationWindow auntificationWindow = new AuntificationWindow(managersFactory.StuffManager);
            var result = auntificationWindow.ShowDialog();
            if(result == true && auntificationWindow.AuthorizedStaff != null)
            {
                Stuff stuff = auntificationWindow.AuthorizedStaff;
                this.Title += $"Администратор: {stuff.ShortName}";
                return true;
            }
            return false;
        }


    }
}