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
    public class ReturnBooksViewModel : ViewModelBase
    {
        public ReturnBooksViewModel(BookHistoryManager bookHistoryManager, RackManager rackManager)
        {
            this.bookHistoryManager = bookHistoryManager;
            this.rackManager = rackManager;
            initData();
        }

        private RackManager rackManager;
        private BookHistoryManager bookHistoryManager;

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                Set(ref _title, value);

            }
        }
        public bool? DialogResult { get; private set; } = null;
        public EventHandler<EventArgs> EndWork { get; set; }

        public ObservableCollection<User> Users { get; private set; } = new();
        public ObservableCollection<Book> Books { get; private set; } = new();

        private int? _selectedUserIndex = null;
        public int? SelectedUserIndex

        {
            get => _selectedUserIndex;
            set { Set(ref  _selectedUserIndex, value); } 
        }
        private int? _selectedBookIndex = null;
        public int? SelectedBookIndex
        {
            get => _selectedBookIndex;
            set { Set(ref _selectedBookIndex, value); }
        }
        private string? _remarks = null;
        public string? Remarks
        {
            get => _remarks;
            set { Set(ref _remarks, value); }
        }

        private ICommand _selectUserCmd;
        public ICommand SelectUserCmd => _selectUserCmd ??=
            new RelayCommand(selectUserExecuted);
        private ICommand _returnBookCmd;
        public ICommand ReturnBookCmd => _returnBookCmd ??=
            new RelayCommand(returnBookExecuted, canReturnBook);
        private ICommand _cancelCmd;
        public ICommand CancelCmd => _cancelCmd ??=
            new RelayCommand(cancelExecuted);

        /// <summary>
        /// Обработчик для команды Отмена.
        /// </summary>
        /// <param name="obj"></param>
        private void cancelExecuted(object obj)
        {
            DialogResult = false;
            EndWork?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Проверяет возможность возврата книги.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool canReturnBook(object arg)
        {
            if (SelectedBookIndex != null && SelectedUserIndex != null) return true;
            return false;
        }

        /// <summary>
        /// Обработчик команды вернуть книгу.
        /// </summary>
        /// <param name="obj"></param>
        private void returnBookExecuted(object obj)
        {
            // Возвращаемая книга.
            Book book = Books[SelectedBookIndex!.Value];
            foreach(BookHistory bookHistory in book.BookHistory)
            {
                if(bookHistory.User == Users[SelectedUserIndex!.Value])
                {
                    // Запись в историю книги даты возврата и примечания
                    bookHistory.ReturnDate = DateTime.Now;
                    bookHistory.Remarks = Remarks;
                    // Постановка книги на стеллаж
                    Rack rack = rackManager.FindRack(r => r.Name.Contains(book.Genre.Name)).
                        OrderBy(r => r.Books.Count).First();
                    bookHistory.Book.Rack = rack;

                    bookHistoryManager.UpdateBookHistory(bookHistory);
                    bookHistoryManager.SaveChanges();
                    break;
                }
            }
            DialogResult = true;
            EndWork?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Обработчик команды выбора читателя.
        /// Загружает коллекцию не  сданных книг читателя.
        /// </summary>
        /// <param name="obj"></param>
        private void selectUserExecuted(object obj)
        {
            if(SelectedUserIndex != null)
            {
                Books.Clear();
                User user = Users[SelectedUserIndex.Value];
                foreach (BookHistory bookHistory in bookHistoryManager.GetBookHistories("Book", "User"))
                {
                    if(bookHistory.ReturnDate == null && bookHistory.User == user)
                        Books.Add(bookHistory.Book);
                }
                if (Books.Count > 0) SelectedBookIndex = 0;
            }
        }

        /// <summary>
        /// Инициализирует начальные данные.
        /// </summary>
        private void initData()
        {
            foreach(BookHistory bookHistory in bookHistoryManager.FindBookHistory(h=>h.ReturnDate == null))
            {
                if(!Users.Contains(bookHistory.User)) Users.Add(bookHistory.User);
            }
        }


    }
}
