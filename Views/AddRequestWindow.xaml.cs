﻿using Library.Business.Managers;
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
    /// Окно добавления заявки
    /// </summary>
    public partial class AddRequestWindow : Window
    {
        AddRequestWindowViewModel addRequestWindowVm;

        public AddRequestWindow(RequestManager requestManager, UserManager userManager,
            BookManager bookManager, BookHistoryManager bookHistoryManager)
        {
            InitializeComponent();

            addRequestWindowVm = new AddRequestWindowViewModel(requestManager, userManager, 
                bookManager, bookHistoryManager);
            addRequestWindowVm.Title = "Добавить запрос";
            this.DataContext = addRequestWindowVm;
        }
    }
}
