using Library.Business.Infastructure;
using Library.Business.Managers;
using Library.Commands;
using Library.Domain.Entities.Books;
using Library.Domain.Entities.Users;
using Library.Views;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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

    public class MainWindowViewModel : ViewModelBase, IDataErrorInfo
    {
        public MainWindowViewModel(ManagersFactory managersFactory)
        {
            mf = managersFactory;
            initManagers();
            initCollections();
        }

        #region Start initialization
        // Инициализация отображаемых данных при запуске программы.
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
        public ObservableCollection<Book> Books { get; set; }
        public ObservableCollection<Request> Requests { get; set; }
        public ObservableCollection<User> Users { get; set; }

        public IEnumerable<RequestStatus> RequestStatusList
        {
            get
            {
                return Enum.GetValues(typeof(RequestStatus)).Cast<RequestStatus>();
            }
        }


        #endregion

        #region Selected elements
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
            if (newBook != null && Books.Contains(newBook))
            {
                Books.Remove(newBook);
                Books.Insert(0, newBook);
                SelectedBook = newBook;
            }
        }

        private ICommand _returnBookCmd;
        public ICommand ReturnBookCmd => _returnBookCmd ??=
            new RelayCommand(returnBookExecuted);

        /// <summary>
        /// Обработчик команды Вернуть книгу.
        /// Открывает окно возврата книги.
        /// </summary>
        /// <param name="obj"></param>
        private void returnBookExecuted(object obj)
        {
            ReturnBooksWindow returnBooksWindow = new(bookHistoryManager, rackManager);
            returnBooksWindow.ShowDialog();

        }

        #endregion

        #region Requests Commands
        private ICommand _addRequestCmd;
        private ICommand _deleteRequestCmd;
        private ICommand _realizeRequestCmd;
        private ICommand _queryFilterCmd;
        public ICommand AddRequestCmd => _addRequestCmd ??=
            new RelayCommand(addRequestExecuted);
        public ICommand DeleteRequestCmd => _deleteRequestCmd ??=
            new RelayCommand(deleteRequestExecuted, (obj) => SelectedRequest != null);
        public ICommand RealizeRequestCmd => _realizeRequestCmd ??=
            new RelayCommand(realizeRequestExecuted, canRealizeRequest);
        public ICommand QueryFilterCmd => _queryFilterCmd ??=
            new RelayCommand(queryFilterExecuted);

        /// <summary>
        /// Обработчик команды создания заявки.
        /// Открывает окно создания заявки.
        /// </summary>
        /// <param name="obj"></param>
        private void addRequestExecuted(object obj)
        {
            AddRequestWindow addRequestWindow = new(
                requestManager, userManager, bookManager, bookHistoryManager
                );

            var result = addRequestWindow.ShowDialog();
            if (result == true)
            {
                updateRequestData();
                Request? newRequest = addRequestWindow.NewRequest;
                if (newRequest != null && Requests.Contains(newRequest))
                    SelectedRequest = newRequest;
            }
        }

        /// <summary>
        /// Обработчик для команды Удалить заявку.
        /// Удаляет заявку, Обновляет данные в ListView.
        /// </summary>
        /// <param name="obj"></param>
        private void deleteRequestExecuted(object obj)
        {
            if (SelectedRequest != null)
            {
                requestManager.DeleteRequest(SelectedRequest);
                requestManager.SaveChanges();
                updateRequestData();
            }
        }

        /// <summary>
        /// Проверяет может ли быть реализована заявка.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool canRealizeRequest(object arg)
        {
            return (SelectedRequest != null &&
                SelectedRequest.Book.Rack != null &&
                SelectedRequest.IssueDate == null);
        }

        /// <summary>
        /// Обработчик для команды реализации заявки.
        /// </summary>
        /// <param name="obj"></param>
        private void realizeRequestExecuted(object obj)
        {
            DateTime currentDate = DateTime.Now;
            BookHistory bookHistory = new(currentDate);
            bookHistory.User = SelectedRequest!.User;
            bookHistory.Book = SelectedRequest.Book;
            bookHistory.Book.Rack = null;
            SelectedRequest.IssueDate = currentDate;

            bookHistoryManager.CreateBookHistory(bookHistory);
            requestManager.UpdateRequest(SelectedRequest);
            requestManager.SaveChanges();
            updateRequestData();

        }

        /// <summary>
        /// Обработчик команды фильтрации заявок.
        /// Отображает заявки согласно выбранному фильтру.
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void queryFilterExecuted(object obj)
        {
            switch (SelectedRequestStatus)
            {
                case RequestStatus.Все:
                    {
                        updateRequestData();
                    }
                    break;
                case RequestStatus.Выданные:
                    {
                        Requests.Clear();
                        SelectedRequest = null;
                        foreach (Request request in requestManager.FindRequest(r => r.IssueDate != null))
                            Requests.Add(request);
                    }
                    break;
                case RequestStatus.Очередь:
                    {
                        Requests.Clear();
                        SelectedRequest = null;
                        foreach (Request request in requestManager.FindRequest(r => r.IssueDate == null))
                            Requests.Add(request);
                    }
                    break;
            }

        }
        #endregion

        #region Users Commands
        private ICommand _addUserCmd;
        private ICommand _deleteUserCmd;

        public ICommand AddUserCmd => _addUserCmd ??=
            new RelayCommand(addUserExecuted);

        public ICommand DeleteUserCmd => _deleteUserCmd ??=
            new RelayCommand(deleteUserExecuted, userSelected);

        /// <summary>
        /// Проверяет выбран ли читатель.
        /// </summary>
        private bool userSelected(object obj)
        {
            return SelectedUser != null;
        }

        /// <summary>
        /// Обработчик команды добавления читателя.
        /// Открывает окно создания профиля читателя.
        /// </summary>
        /// <param name="obj"></param>
        private void addUserExecuted(object obj)
        {
            AddUserWindow addUserWindow = new(userManager);
            var result = addUserWindow.ShowDialog();
            if (result == true)
            {
                updateUserData();
                User? newUser = addUserWindow.NewUser;
                if (newUser != null && Users.Contains(newUser))
                    SelectedUser = newUser;
            }
        }

        /// <summary>
        /// Удаляет читателя.
        /// </summary>
        /// <param name="obj"></param>
        private void deleteUserExecuted(object obj)
        {
            if (SelectedUser != null)
            {
                //Проверяет перед удалением сданы ли у читателя книги.
                List<BookHistory> bookHistory = new();
                bookHistory = bookHistoryManager.FindBookHistory(h => h.User == SelectedUser && h.ReturnDate == null).ToList();
                if (bookHistory.Count > 0)
                {
                    string booksName = string.Empty;
                    foreach (BookHistory bookHistoryItem in bookHistory)
                        booksName += bookHistoryItem.Book.Name + "\n";

                    string msg = $"Невозможно удалить читателя {SelectedUser.FullName}\n" +
                        $"У читателя не сданы книги:\n{booksName}";
                    MessageBox.Show(msg, "Удаление", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Если книги сданы запрос подтверждения на удаления читателя
                    var result = MessageBox.Show($"Удалить читателя\n{SelectedUser.FullName}?",
                    "Удалить читателя",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                    //Удаление читателя
                    if (result == MessageBoxResult.Yes)
                    {
                        userManager.DeleteUser(SelectedUser);
                        userManager.SaveChanges();
                        SelectedUser = null;
                        updateUserData();
                        updateRequestData();
                    }
                }

            }
        }
        #endregion

        #region Diagrams
        private List<String> chartsVariants = new()
        { "Количество книг, по годам издания",
          "Количество книг, выданных за определенный период",
          "Суммарное количество страниц по годам издания"
        };
        public List<String> ChartsVariants
        {
            get => chartsVariants;
        }
        private int _selectedChartVariantIndex = 0;
        public int SelectedChartVariantIndex
        {
            get { return _selectedChartVariantIndex; }
            set
            {
                Set(ref _selectedChartVariantIndex, value);
            }
        }
        private string _diagramTitle = string.Empty;
        public string DiagramTitle
        {
            get { return _diagramTitle; }
            set { Set(ref _diagramTitle, value); }
        }

        //Diagram Binding
        public SeriesCollection Series { get; set; } = new SeriesCollection();
        private string _titleAxisX = string.Empty;
        public string TitleAxisX
        {
            get => _titleAxisX;
            set { Set(ref _titleAxisX, value); }
        }
        private string _titleAxisY = string.Empty;
        public string TitleAxisY
        {
            get => _titleAxisY;
            set { Set(ref _titleAxisY, value); }
        }
        //X
        private int _axisXMinValue = 0;
        public int AxisXMinValue
        {
            get => _axisXMinValue;
            set { Set(ref _axisXMinValue, value); }
        }
        private int _axisXMaxValue = 10;
        public int AxisXMaxValue
        {
            get => _axisXMaxValue;
            set { Set(ref _axisXMaxValue, value); }
        }
        private int _sepStepX = 10;
        public int SepStepX
        {
            get => _sepStepX;
            set { Set(ref _sepStepX, value); }
        }
        private Func<double, string> _xAxisLabelFormatter;
        public Func<double, string> XAxisLabelFormatter
        {
            get => _xAxisLabelFormatter;
            set { Set(ref _xAxisLabelFormatter, value); }
        }
        //Y
        private int _axisYMinValue = 0;
        public int AxisYMinValue
        {
            get => _axisYMinValue;
            set { Set(ref _axisYMinValue, value); } // Исправлено
        }
        private int _axisYMaxValue = 10;
        public int AxisYMaxValue
        {
            get => _axisYMaxValue;
            set { Set(ref _axisYMaxValue, value); }
        }
        private int _sepStepY = 1;
        public int SepStepY
        {
            get => _sepStepY;
            set { Set(ref _sepStepY, value); }
        }

        private DateTime _dateDiagramEnd = DateTime.Now;
        public DateTime DateDiagramEnd
        {
            get { return _dateDiagramEnd; }
            set { Set(ref _dateDiagramEnd, value); }
        }

        private DateTime _dateDiagramStart = DateTime.Now.AddDays(-10);
        public DateTime DateDiagramStart
        {
            get { return _dateDiagramStart; }
            set { Set(ref _dateDiagramStart, value); }
        }

        private DateTime _minDateDiagram = DateTime.Now.AddYears(-20);
        public DateTime MinDateDiagram
        {
            get => _minDateDiagram;
            set { Set(ref _minDateDiagram, value); }
        }

        private DateTime _maxDateDiagram = DateTime.Now;
        public DateTime MaxDateDiagram
        {
            get => _maxDateDiagram;
            set { Set(ref _maxDateDiagram, value); }
        }

        private bool _dateDiagramError = false;
        public bool DateDiagramError
        {
            get => _dateDiagramError;
            set { Set(ref _dateDiagramError, value); }
        }

        private Visibility _visibilityDate = Visibility.Collapsed;
        public Visibility VisibilityDate
        {
            get { return _visibilityDate; }
            set { Set(ref _visibilityDate, value); }
        }

        private ICommand _selectDiagramCmd;
        public ICommand SelectDiagramCmd => _selectDiagramCmd ??=
            new RelayCommand(selectDiagramExecuted);

        /// <summary>
        /// Обработчик команды выбора варианта диаграммы.
        /// </summary>
        /// <param name="obj"></param>
        private void selectDiagramExecuted(object obj)
        {
            updateDateDiagram();
            // DatePicker Видны только при выборе диаграммы номер 2.
            if (SelectedChartVariantIndex == 1) VisibilityDate = Visibility.Visible;
            else
                VisibilityDate = Visibility.Collapsed;
        }

        private ICommand _showDiagramCmd;
        public ICommand ShowDiagramCmd => _showDiagramCmd ??=
            new RelayCommand(showDiagrammExecuted, canShowDiagram);

        /// <summary>
        /// Определяет возможность отображения диаграммы.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool canShowDiagram(object arg)
        {
            if (SelectedChartVariantIndex == 1 && DateDiagramError == true) return false;
            return true;
        }

        /// <summary>
        /// Обработчик команды показать диаграмму.
        /// </summary>
        private void showDiagrammExecuted(object obj)
        {
            DiagramTitle = ChartsVariants[SelectedChartVariantIndex];
            Series.Clear();

            List<Book> books = bookManager.GetBooks().ToList();
            // Обработка выбора варианта диаграммы.
            switch (SelectedChartVariantIndex)
            {
                case 0:
                    {
                        // Группируем книги по годам издания
                        var booksByYear = books.GroupBy(b => b.PublicationDate)
                                               .Select(g => new
                                               {
                                                   Year = g.Key,
                                                   Count = g.Count()
                                               })
                                               .OrderBy(b => b.Year)
                                               .ToList();
                        if (booksByYear.Any())
                        {
                            XAxisLabelFormatter = value => value.ToString("0");
                            TitleAxisX = "Года";
                            TitleAxisY = "Количество книг";
                            AxisXMinValue = booksByYear.First().Year - 50;
                            AxisXMaxValue = booksByYear.Last().Year + 2;
                            AxisYMinValue = 0;
                            AxisYMaxValue = booksByYear.Last().Count + 2;
                            SepStepX = 25;
                            SepStepY = 1;

                            ChartValues<ObservablePoint> chartValues = new();
                            foreach (var book in booksByYear)
                            {
                                chartValues.Add(new ObservablePoint(book.Year, book.Count));
                            }

                            LineSeries lineSeries = new()
                            {
                                Title = "Количество книг",
                                Values = chartValues,
                                PointGeometry = DefaultGeometries.Circle,
                                PointGeometrySize = 10,
                                Stroke = Brushes.Blue,
                                Fill = Brushes.Transparent

                            };
                            Series.Add(lineSeries);
                        }
                        break;
                    }

                case 1:
                    {// Получаем истории выдач книг за указанный период
                        List<BookHistory> bookHistory = bookHistoryManager.FindBookHistory(
                            bh => bh.IssueDate >= DateDiagramStart && bh.IssueDate <= DateDiagramEnd
                        ).ToList();

                        // Группируем по дате выдачи и считаем количество книг, выданных в каждый день
                        // Используем .Date, чтобы группировать по дням
                        var booksByDate = bookHistory.GroupBy(bh => bh.IssueDate.Date)
                            .Select(g => new
                            {
                                Date = g.Key,
                                TotalBooks = g.Count()
                            })
                            .OrderBy(b => b.Date)
                            .ToList();

                        if (booksByDate.Any())
                        {
                            XAxisLabelFormatter = value => DateTime.FromOADate(value).ToString("dd.MM.yyyy");
                            TitleAxisX = "Дата";
                            TitleAxisY = "Количество книг";
                            AxisXMinValue = (int)DateDiagramStart.Date.ToOADate();
                            AxisXMaxValue = (int)DateDiagramEnd.Date.ToOADate();
                            AxisYMinValue = 0;
                            AxisYMaxValue = booksByDate.Max(b => b.TotalBooks) + 5;
                            SepStepX = (AxisXMaxValue - AxisXMinValue) / 5;
                            SepStepY = 1;

                            ChartValues<ObservablePoint> chartValues = new();
                            foreach (var book in booksByDate)
                            {
                                chartValues.Add(new ObservablePoint(book.Date.ToOADate(), book.TotalBooks));
                            }

                            LineSeries lineSeries = new()
                            {
                                Title = "Количество выданных книг",
                                Values = chartValues,
                                PointGeometry = DefaultGeometries.Circle,
                                PointGeometrySize = 10,
                                Stroke = Brushes.Green,
                                Fill = Brushes.Transparent
                            };

                            Series.Add(lineSeries);
                        }

                        break;
                    }

                case 2:
                    {
                        // Группируем книги по годам издания и суммируем количество страниц
                        var pagesByYear = books.GroupBy(b => b.PublicationDate)
                                               .Select(g => new
                                               {
                                                   Year = g.Key,
                                                   TotalPages = g.Sum(b => b.NumberPages)
                                               })
                                               .OrderBy(b => b.Year)
                                               .ToList();

                        if (pagesByYear.Any())
                        {
                            XAxisLabelFormatter = value => value.ToString("0");
                            TitleAxisX = "Года";
                            TitleAxisY = "Суммарное количество страниц";
                            AxisXMinValue = pagesByYear.First().Year - 50;
                            AxisXMaxValue = pagesByYear.Last().Year + 2;
                            AxisYMinValue = 0;
                            AxisYMaxValue = pagesByYear.Max(b => b.TotalPages) + 100;
                            SepStepX = 25;
                            SepStepY = 100;

                            ChartValues<ObservablePoint> chartValues = new();
                            foreach (var book in pagesByYear)
                            {
                                chartValues.Add(new ObservablePoint(book.Year, book.TotalPages));
                            }

                            LineSeries lineSeries = new()
                            {
                                Title = "Суммарное количество страниц",
                                Values = chartValues,
                                PointGeometry = DefaultGeometries.Circle,
                                PointGeometrySize = 10,
                                Stroke = Brushes.Green,
                                Fill = Brushes.Transparent
                            };

                            Series.Add(lineSeries);
                        }

                        break;
                    }

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
            foreach (Book book in bookManager.GetBooks()) Books.Add(book);
            if (tempSelectedBook != null && Books.Contains(tempSelectedBook))
                SelectedBook = tempSelectedBook;
        }

        /// <summary>
        /// Обновляет коллекцию заявок.
        /// </summary>
        private void updateRequestData()
        {
            Request? tempSelectedRequest = SelectedRequest;
            Requests.Clear();
            SelectedRequest = null;
            foreach (Request request in requestManager.GetRequests()) Requests.Add(request);
            if (tempSelectedRequest != null && Requests.Contains(tempSelectedRequest))
                SelectedRequest = tempSelectedRequest;
        }

        /// <summary>
        /// Обновляет коллекцию читателей.
        /// </summary>
        private void updateUserData()
        {
            User? tempSelectedUser = SelectedUser;
            Users.Clear();
            SelectedUser = null;
            foreach (User user in userManager.GetUsers()) Users.Add(user);
            if (tempSelectedUser != null && Users.Contains(tempSelectedUser))
                SelectedUser = tempSelectedUser;
        }

        private void updateDateDiagram()
        {
            DateDiagramStart = DateTime.Now.AddDays(-10);
            DateDiagramEnd = DateTime.Now;
        }
        #endregion

        #region Validation

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(DateDiagramStart):
                        {
                            if (DateDiagramStart > DateDiagramEnd)
                            {
                                error = "Дата начала не может быть позже даты окончания периода!";
                                DateDiagramError = true;
                            }
                            else
                            {
                                error = string.Empty;
                                DateDiagramError = false;
                            }
                            break;
                        }
                    case nameof(DateDiagramEnd):
                        {
                            if (DateDiagramEnd < DateDiagramStart)
                            {
                                error = "Дата окончания периода не может быть раньше даты начала!";
                                DateDiagramError = true;
                            }
                            else
                            {
                                error = string.Empty;
                                DateDiagramError = false;
                            }
                            break;
                        }
                }
                return error;
            }
        }
        #endregion
    }

}
