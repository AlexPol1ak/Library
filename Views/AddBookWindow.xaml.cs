using Library.Business.Managers;
using Library.Domain.Entities.Books;
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
    /// Окно добавления книги.
    /// </summary>
    public partial class AddBookWindow : Window
    {
        private AddBookWindowViewModel addBookViewModel;
        // Новая книга, если она была добавлена
        public Book? NewBook { get; private set; } = null;

        public AddBookWindow(AuthorManager authorManager, BookManager bookManager, 
            GenreManager genreManager, TermManager termManager, RackManager rackManager)
        {
            InitializeComponent();
            this.Title = "Добавить книгу";
            addBookViewModel = new(authorManager, bookManager, genreManager, termManager, rackManager);
            addBookViewModel.EndWork += AddBookViewModel_EndWork;
            this.DataContext = addBookViewModel;
        }

        /// <summary>
        /// Обработчик события завершения работы окна.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBookViewModel_EndWork(object? sender, EventArgs e)
        {
            AddBookWindowViewModel wndViewModel = ((AddBookWindowViewModel)sender!);
            this.DialogResult = wndViewModel.DialogResult;
            this.NewBook = wndViewModel.NewBook;
            this.Close();
        }
    }
}
