using Library.Business.Infastructure;
using Library.DatabaseСonnection;
using System.Windows;
using System.Windows.Controls;

namespace Library.Views
{

    /// <summary>
    /// Окно выбора сервера
    /// </summary>
    public partial class ChooseServerWindow : Window
    {
        private Dictionary<string, Dictionary<string, string>> _serverData = DbConnectionsData.ServerData;
        public ManagersFactory? ManagersFactory { get; private set; } = null;


        public ChooseServerWindow()
        {
            InitializeComponent();
            initData();
        }

        /// <summary>
        /// Инициализация компонентов.
        /// </summary>
        private void initData()
        {
            foreach (string serverName in _serverData.Keys) Cb_Servers.Items.Add(serverName);
            Cb_Servers.SelectedIndex = 0;
            btn_EntryServerIsEnabled();
        }

        /// <summary>
        /// Обработчик нажатия кнопки подключиться.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn_EntryServer_Click(object sender, RoutedEventArgs e)
        {
            string selectedServerName = Cb_Servers.Text;
            string inputPass = PassBox_ServerPass.Password;

            Dictionary<string, string> serverJsonKeys = _serverData[selectedServerName];
            string connectionName = serverJsonKeys["ConnectionName"];
            string sqlVersion = serverJsonKeys["SqlVersion"];
            string serverAccessPass = serverJsonKeys["pass"];

            LoadingProgressBar.Visibility = Visibility.Visible;
            Btn_EntryServer.IsEnabled = false;
            Btn_CancelServer.IsEnabled = false;

            bool isConnected = false;
            try
            {
                if (selectedServerName == "Local")
                {
                    // Локальное подключение не требует пароля
                    isConnected = await Task.Run(() => createManagersFactory(connectionName, sqlVersion));
                }
                else
                {
                    // Проверяем пароль.
                    if (inputPass == serverAccessPass)
                    {
                        isConnected = await Task.Run(() => createManagersFactory(connectionName, sqlVersion));
                    }
                    else
                    {
                        PassBox_ServerPass.Password = string.Empty;
                        Tb_Info.Text = "Неверный пароль!";
                    }
                }
                // Проверка соединения
                if (isConnected)
                {
                    DialogResult = true;
                    this.Close();
                }
                //Если пароль введен верно но подключение к серверу не установлено.
                else if (inputPass == serverAccessPass)
                {
                    Tb_Info.Text = "Сервер недоступен!";
                }
            }
            // Другие ошибки.
            catch (Exception ex)
            {
                Tb_Info.Text = $"Ошибка подключения: {ex.Message}";
            }
            finally
            {
                LoadingProgressBar.Visibility = Visibility.Hidden;
                Btn_EntryServer.IsEnabled = true;
                Btn_CancelServer.IsEnabled = true;
            }

            btn_EntryServerIsEnabled();
        }

        /// <summary>
        /// Метод подключения к серверу создания фабрики менеджеров
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <param name="mySqlVersionStringName"></param>
        /// <returns></returns>
        private bool createManagersFactory(string connectionStringName, string mySqlVersionStringName)
        {
            try
            {
                this.ManagersFactory = new ManagersFactory(connectionStringName, mySqlVersionStringName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки Отмена.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_CancelServer_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.ManagersFactory = null;
            this.Close();
        }

        private void ServerChangedExecuted(object sender, SelectionChangedEventArgs e)
        {
            PassBox_ServerPass.Password = string.Empty;
            if (Cb_Servers.SelectedIndex == 0) PassBox_ServerPass.IsEnabled = false;
            else
            {
                PassBox_ServerPass.IsEnabled = true;
            }
            btn_EntryServerIsEnabled();
            Tb_Info.Text = string.Empty;

        }

        /// <summary>
        /// Обработчик события ввода пароля.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PassInputExecuted(object sender, RoutedEventArgs e)
        {
            btn_EntryServerIsEnabled();
            Tb_Info.Text = string.Empty;
        }


        /// <summary>
        /// Метод контроля доступности кнопки подключиться.
        /// </summary>
        private void btn_EntryServerIsEnabled()
        {
            if (Cb_Servers.SelectedIndex != 0)
            {
                if (PassBox_ServerPass.Password.Count() < 2) Btn_EntryServer.IsEnabled = false;
                else
                {
                    Btn_EntryServer.IsEnabled = true;
                }
            }
            else { Btn_EntryServer.IsEnabled = true; }
        }

    }
}
