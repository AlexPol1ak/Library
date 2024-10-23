using Library.Business.Managers;
using Library.Domain.Entities.Users;
using Library.Domain.Utils;
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
    /// Окно аутентификации.
    /// </summary>
    public partial class AuntificationWindow : Window
    {
        private StuffManager stuffManager;
        private int attemptСounter = 0;

        public Stuff? AuthorizedStaff { get; private set; } = null;

        public AuntificationWindow( StuffManager stuffManager)
        {
            InitializeComponent();
            this.stuffManager = stuffManager;
        }

        /// <summary>
        /// Обработчик для события ввода email  и пароль.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputFields_TextChanged(object sender, RoutedEventArgs e)
        {
            Btn_Entry.IsEnabled = !string.IsNullOrWhiteSpace(TextBox_Email.Text)
                && !string.IsNullOrWhiteSpace(PassBox_Pass.Password);

            TextBlock_Info.Text = string.Empty;
            TextBox_Email.BorderBrush = Brushes.Transparent;
        }

        /// <summary>
        /// Обработчик нажатия кнопки Войти
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Entry_Click(object sender, RoutedEventArgs e)
        {
            TextBlock_Info.Text = string.Empty;
            ;

            string email = TextBox_Email.Text;
            string password = PassBox_Pass.Password;

            List<Stuff>stuffs = stuffManager.FindStuff(s=>s.Email == email).ToList();
            if(stuffs.Count < 1)
            {
                TextBlock_Info.Text = "Такой email не зарегистрирован!";
                TextBox_Email.BorderBrush = Brushes.Red;               
            }
            else
            {
                Stuff stuff = stuffs[0];
                bool result = stuff.VerifyPassword(password);               
                if (result == true)
                {
                    this.DialogResult = true;
                    AuthorizedStaff = stuff;
                    this.Close();
                }
                else
                {                    
                    PassBox_Pass.BorderBrush = Brushes.Red;
                    PassBox_Pass.Password = string.Empty;
                    TextBlock_Info.Text = "Неверный пароль!";
                }
            }
            attempControl(3);
        }

        /// <summary>
        /// Обработчик нажатия кнопки Отмена
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Метод контроля количества попыток ввода пароля
        /// </summary>
        private void attempControl(int limit)
        {
            attemptСounter++;

            if (attemptСounter > limit)
            {
                this.DialogResult = false;
                this.Close();
            }
            TextBlock_Info.Text += $"\nОсталось попыток: {limit - attemptСounter}";
        }
    }
}
