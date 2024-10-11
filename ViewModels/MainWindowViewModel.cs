using Library.Business.Infastructure;
using Library.Business.Managers;
using Library.Commands;
using Library.Domain.Entities.Books;
using Library.Domain.Entities.Users;
using Library.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Library.ViewModels
{
    /// <summary>
    ///  Статусы заявок.
    /// </summary>
    public enum RequestStatus
    {
        Все,
        Очередь,
        Выданные
    }

    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel( ManagersFactory managersFactory)
        {
            mf = managersFactory;
            initManagers();
            initCollections();
        }

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
        // Коллекции привязок в главном окне
        public ObservableCollection<Book> Books {  get; set; }
        public ObservableCollection<Request> Requests { get; set; }
        public ObservableCollection<User> Users { get; set; }

        public IEnumerable<RequestStatus> RequestStatusList
        {
            get
            {
                return Enum.GetValues(typeof(RequestStatus)).Cast<RequestStatus>();
            }
        }

        private List<String> chartsVariants = new()
        { "Количество книг, по годам издания", 
          "Количество книг, выданных за определенный период",
          "Cуммарное количество страниц по годам издания"
        };
        public IEnumerable<String> ChartsVariants
        {
            get => chartsVariants;           
        }
        #endregion

        #region Selected
        // Выбор пользователя 
        private Book? _selectedBook = null;
        public Book? SelectedBook
        {
            get => _selectedBook;
            set { Set(ref _selectedBook, value); }
        }

        private Request? _selectedRequest = null;
        public Request? SelectedRequest
        {
            get => _selectedRequest;
            set { Set(ref _selectedRequest, value); } 
        }

        private RequestStatus _selectedRequestStatus = RequestStatus.Все;
        public RequestStatus SelectedRequestStatus
        {
            get { return _selectedRequestStatus; }
            set
            {
                _selectedRequestStatus = value;
                Set(ref _selectedRequestStatus, value);
            }
        }
      
        private User? _selectedUser = null;
        public User? SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                Set(ref _selectedUser, value);
            }
        }

        private int _selectedChartVariantIndex = 0;
        public int SelectedChartVariantIndex
        {
            get { return _selectedChartVariantIndex; }
            set
            {
                _selectedChartVariantIndex = value;
                Set(ref _selectedChartVariantIndex, value);
            }
        }
        #endregion
   
        #region Start initialization
        // Инициализация отобржаемых данных при запуске программы.
        /// <summary>
        /// Инициализация менеджеров.
        /// </summary>
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

        /// <summary>
        /// Начальная инициализация коллекций
        /// </summary>
        private void initCollections()
        {

            Books = new ObservableCollection<Book>(bookManager.GetBooks("Authors", "Genre", "Rack"));
            Requests = new ObservableCollection<Request>(requestManager.GetRequests("User", "Book"));
            Users = new ObservableCollection<User>(userManager.GetUsers("Requests"));
        }
        #endregion

        #region Commands
        #region Books Commands
        // Команда удалить книгу.
        private ICommand _deleteBookCmd;
        public ICommand DeleteBookCmd => _deleteBookCmd ??=
            new RelayCommand(
                deleteBookExecuted,
                (id) => SelectedBook != null
                );

        /// <summary>
        /// Обработчик команды Удалить книгу
        /// </summary>
        /// <param name="obj"></param>
        private void deleteBookExecuted(object obj)
        {
            Book? book = obj as Book;
            if (book != null)
            {
                var result = MessageBox.Show(
                    $"Удалить книгу\n{book.Name}?",
                    "Удалить книгу",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No
                    );
                if (result == MessageBoxResult.Yes)
                {
                    bookManager.DeleteBook(book.BookId);
                    bookManager.SaveChanges();
                }            
            }
            updateBookData();
        }
        // Команда добавить книгу.
        private ICommand _addBookCmd;
        public ICommand AddBookCmd => _addBookCmd ??=
            new RelayCommand(addBookExecuted);
        /// <summary>
        /// Обработчик команды добавить книгу.
        /// </summary>
        private void addBookExecuted(object obj)
        {
            AddBookWindow addBookWindow = new(authorManager, bookManager,
                genreManager, termManager, rackManager);
            addBookWindow.ShowDialog();
            Book? newBook = addBookWindow.NewBook;
            updateBookData();
            if(newBook != null && Books.Contains(newBook))
            {
                Books.Remove(newBook);
                Books.Insert(0, newBook);
                SelectedBook = newBook;                
            }            
        }
        #endregion

        #region Requests Commands
        private ICommand _addRequestCmd;
        private ICommand _deleteRequestCmd;
        private ICommand _realizationRequestCmd;
        public ICommand AddRequestCmd => _addRequestCmd ??= 
            new  RelayCommand(addRequestExecuted);

        /// <summary>
        /// Обработчик для команды Добавить заявку.
        /// Добавляет заявку на выдачу книги.
        /// </summary>
        private void addRequestExecuted(object obj)
        {
            AddRequestWindow addRequestWindow = new(
                requestManager, userManager, bookManager, bookHistoryManager
                );

            var result = addRequestWindow.ShowDialog();
            if(result == true)
            {
                updateRequestData();
                Request? newRequest = addRequestWindow.NewRequest;
                if (newRequest != null && Requests.Contains(newRequest))
                    SelectedRequest = newRequest;               
            }
        }
        #endregion
        #endregion

        #region Supporting Methods
        /// <summary>
        /// Обновляет коллекцию книг.
        /// </summary>
        private void updateBookData()
        {
            Book? tempSelectedBook = SelectedBook;
            Books.Clear();
            SelectedBook = null;
            foreach(Book book in bookManager.GetBooks())Books.Add(book);
            if(tempSelectedBook != null && Books.Contains(tempSelectedBook)) 
                SelectedBook = tempSelectedBook;
        }

        /// <summary>
        /// Обновляет коолекцию заявок.
        /// </summary>
        private void updateRequestData()
        {
            Request? tempSelectedRequest = SelectedRequest;
            Requests.Clear();
            SelectedRequest = null;
            foreach (Request request in requestManager.GetRequests())Requests.Add(request);
            if(tempSelectedRequest != null && Requests.Contains(tempSelectedRequest))
                SelectedRequest = tempSelectedRequest;
            
        }
        #endregion
    }

}
