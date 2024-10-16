﻿using Library.Business.Infastructure;
using Library.Business.Infastructure.DbFakeData;
using Library.ViewModels;
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
            managersFactory =  new ManagersFactory("DefaultConnection", "MySQLVersion");
            //if (new FakeData(managersFactory).InstallData() is bool flag) MessageBox.Show($"Установка начальных данных: {flag}");

            mainWindowViewModel = new (managersFactory);
            this.DataContext = mainWindowViewModel;
                    
        }
      
    }
}