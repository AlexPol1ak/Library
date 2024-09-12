using Library.Business.Infastructure;
using Library.Business.Managers;
using Library.Domain.Entities.Books;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Library.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Managers
        ManagersFactory mf;
        private AuthorManager authorManager;
        private BookManager bookManager;
        private BookHistoryManager bookHistoryManager;
        private GenreManager genreManager;
        private RackManager rackManager;
        private RequestManager requestManager;
        private StuffManager stuffManager;
        private UserManager userManager;
        private TermManager termManager;
        #endregion

        #region Collections
        public ObservableCollection<Book> Books {  get; set; }
        #endregion

        #region Selected
        private Book? _selectedBook = null;
        public Book? SelectedBook
        {
            get => _selectedBook;
            set { Set(ref _selectedBook, value); }
        }
        #endregion

        public MainWindowViewModel(ManagersFactory managersFactory)
        {
            mf = managersFactory;
            initManagers();
            initCollections();
        }

        private void initManagers()
        {
            authorManager = mf.AuthorManager;
            bookManager = mf.BookManager;
            bookHistoryManager = mf.BookHistoryManager;
            genreManager = mf.GenreManager;
            rackManager = mf.RackManager;
            requestManager = mf.RequestManager;
            stuffManager = mf.StuffManager;
            userManager = mf.UserManager;
            termManager = mf.TermManager;
        }

        private void initCollections()
        {

            Books = new ObservableCollection<Book>(bookManager.GetBooks("Authors", "Genre", "Rack"));
        }

    }
}
