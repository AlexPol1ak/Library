using Library.Business.Managers;
using Library.Commands;
using Library.Domain.Entities.Books;
using Library.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Library.ViewModels
{
    public class AddRequestWindowViewModel : ViewModelBase
    {
        #region Data
        public EventHandler<EventArgs> EndWork;
        public bool? DialogResult { get; private set; } = false;
        public Request? NewRequest { get; private set; } = null;

        private RequestManager requestManager;
        private UserManager userManager;
        private BookManager bookManager;
        private BookHistoryManager bookHistoryManager;
        public string Title { get; set; }

        private ObservableCollection<User> _users = new();
        private ObservableCollection<Book> _books = new();
        private int? _selectedUserIndex = null;
        private int? _selectedBookIndex = null;
        private string _infoText = string.Empty;
        public ObservableCollection<User> Users
        {
            get => _users;
            set { Set(ref _users, value); }
        }
        public ObservableCollection<Book> Books
        {
            get => _books;
            set { Set(ref _books, value); }
        }
        public int? SelectedUserIndex
        {
            get => _selectedUserIndex;
            set { Set(ref _selectedUserIndex, value); }
        }
        public int? SelectedBookIndex
        {
            get => _selectedBookIndex;
            set { Set(ref _selectedBookIndex, value); }
        }
        public string InfoText
        {
            get => _infoText;
            set { Set(ref _infoText, value); }
        }
        #endregion

        public AddRequestWindowViewModel(RequestManager requestManager, UserManager userManager,
            BookManager bookManager, BookHistoryManager bookHistoryManager)
        {
            this.requestManager = requestManager;
            this.userManager = userManager;
            this.bookManager = bookManager;
            this.bookHistoryManager = bookHistoryManager;
            initData();           
        }

        /// <summary>
        /// Инициализирует данные окна.
        /// </summary>
        private void initData()
        {
            List<User> tempUsers = userManager.GetUsers("Requests").OrderBy(u=>u.FirstName).ToList();    
            foreach (User user in tempUsers) Users.Add(user);

            List<Book> tempBooks = bookManager.GetBooks("Genre", "Rack", "Term", "BookHistory").
                OrderBy(b => b.Name).ToList();
            foreach (Book book in tempBooks) Books.Add(book);

            SelectedBookIndex = null;
            SelectedUserIndex = null ;

            InfoText = "Информация о заявке";
        }

        /// <summary>
        /// Команда сохранения заявки
        /// </summary>
        private ICommand _saveCmd;
        public ICommand SaveCmd => _saveCmd ??=
            new RelayCommand(
                addRequestExecuted,
                canAddRequestExecuted
                );

        /// <summary>
        /// Проверяет возможность сохранения заявки на книгу.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool canAddRequestExecuted(object arg)
        {
            if(SelectedUserIndex == null || SelectedBookIndex == null ||
                !checkUserLimit(Users[SelectedUserIndex.Value], out int numberBooks))
                return false;        

            return true;
        }
        /// <summary>
        /// Обрабочтик команды сохранения заявки на книгу.
        /// </summary>
        /// <param name="obj"></param>
        private void addRequestExecuted(object obj)
        {

            Book book = Books[SelectedBookIndex.Value];
            User user = Users[SelectedUserIndex.Value];
            DateTime dateNow = DateTime.Now;
            var result = AddRequestWindowViewModel.checkReturnBook(book);
            Request newRequest = new(user, book, dateNow);

            if(result == null)
            {
                book.Rack = null;
                newRequest.IssueDate = dateNow;
                BookHistory bookHistory = new ();
                bookHistory.IssueDate = dateNow;
                bookHistory.User = user;
                bookHistory.Book = book;
                requestManager.CreateRequest(newRequest);
                bookHistoryManager.CreateBookHistory(bookHistory);
                bookManager.UpdateBook(book);

                bookManager.SaveChanges();

                DialogResult = true;
                NewRequest = newRequest;
                EndWork?.Invoke(this, EventArgs.Empty);

            }
            else
            {
                DateOnly returnDate = result.Value;
                string msg = $"Книга {book.Name} находится у читателя.\n" +
                    $"Будет возвращена приблизительно {returnDate.ToShortDateString()}\n" +
                    $"Заявка будет помещена в очередь";

                var resultMb = MessageBox.Show(msg, "Оповещение",
                    MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if(resultMb == MessageBoxResult.OK)
                {
                    newRequest.IssueDate = null;
                    requestManager.CreateRequest(newRequest) ;                   

                    NewRequest = newRequest;
                    DialogResult = true;
                    EndWork?.Invoke(this, EventArgs.Empty);
                }
            }                   
        }

        /// <summary>
        /// Команда отмены созданиния заявки.
        /// </summary>
        private ICommand _cancelCmd;
        public ICommand CancelCmd => _cancelCmd ??=
            new RelayCommand(cancelExecuted);

        /// <summary>
        /// Обработчик команды отмены создания заявки
        /// </summary>
        /// <param name="obj"></param>
        private void cancelExecuted(object obj)
        {
            DialogResult = false;
            EndWork?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Команды выбора для события выбора читателя
        /// </summary>
        private ICommand _selectedUserCmd;
        public ICommand SelectUserCmd => _selectedUserCmd ??=
            new RelayCommand(selectUserExecuted);

        /// <summary>
        /// Обрабочтик команды выбора читателя
        /// </summary>
        /// <param name="obj"></param>
        private void selectUserExecuted(object obj)
        {
            if (SelectedUserIndex != null)
            {
                User user = Users[SelectedUserIndex.Value];
                int numberBooks;
                bool result = AddRequestWindowViewModel.checkUserLimit(user, out numberBooks);

                if (result)
                {
                    if (numberBooks == 0)
                        InfoText = $"{user.FullName}:\nУ читателя нет несданных книг.";
                    else
                        InfoText = $"{user.FullName}:\nНесданных книг — {numberBooks}.";
                }
                else
                    InfoText = $"{user.FullName}:\nНесданных книг — {numberBooks}.\nВыдача новых книг запрещена.";
            }
        }

        /// <summary>
        /// Проверяет, можно ли читателю выдать книгу
        /// </summary>
        /// <param name="user">Читатель</param>
        /// <param name="numberBooks">Количество не сданных книг у читателя</param>
        /// <param name="maxBook">Максимальное количество книг не сданных книг у читателя</param>
        /// <returns>true-если количесто не сданнных некиг не привыщает максимальное допустимое значение, иначе false.</returns>
        protected static bool checkUserLimit(User user, out int numberBooks, int maxBook = 3)
        {
            int counter = 0;
            
            foreach(Request request in user.Requests.Where(r=>r.IssueDate != null))
            {
                foreach(BookHistory bookHistory in request.Book.BookHistory.Where(bh => bh.User == user))
                {
                    if(bookHistory.ReturnDate == null) counter++;
                }
            }

            numberBooks = counter;

            if (counter >= maxBook) return false;        

            return true;
        }

        /// <summary>
        /// Возвращает максимальную дату возврата книги.
        /// </summary>
        /// <param name="book">Книга</param>
        /// <returns>null-Если книга на полке или дата возврата книги. </returns>
        protected static DateOnly? checkReturnBook(Book book)
        {
            if(book.Rack != null) return null;

            if(book.BookHistory.Count > 0)
            {
                foreach(BookHistory bookHistory in book.BookHistory)
                {
                    if(bookHistory.ReturnDate == null )
                    {
                        int? limitDay = bookHistory.Book.Term.MaximumDays;
                        if(limitDay == null) return DateOnly.FromDateTime(DateTime.Now);
                        DateOnly returnDate = DateOnly.FromDateTime(bookHistory.IssueDate.AddDays(limitDay.Value));
                        return returnDate;
                    }
                }
            }

            return DateOnly.MaxValue;
        }

        

    }
}
