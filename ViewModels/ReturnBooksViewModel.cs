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
using System.Windows.Input;

namespace Library.ViewModels
{
    public class ReturnBooksViewModel : ViewModelBase
    {
        public ReturnBooksViewModel(BookHistoryManager bookHistoryManager)
        {
            this.bookHistoryManager = bookHistoryManager;
            initData();
        }

        private UserManager userManager;
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
