using Library.Business.Managers;
using Library.Domain.Entities.Books;
using Library.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.ViewModels
{
    public class AddRequestWindowViewModel : ViewModelBase
    {
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

        public AddRequestWindowViewModel(RequestManager requestManager, UserManager userManager,
            BookManager bookManager, BookHistoryManager bookHistoryManager)
        {
            this.requestManager = requestManager;
            this.userManager = userManager;
            this.bookManager = bookManager;
            this.bookHistoryManager = bookHistoryManager;
        }

        private void initData()
        {
            foreach(User user in userManager.GetUsers("Requests")) Users.Add(user);
            foreach(Book book in bookManager.GetBooks("Genre", "Rack", "Term", "BookHistory"))
                Books.Add(book);
            if (Users.Count > 0) SelectedUserIndex = 0;
            if (Books.Count > 0) SelectedBookIndex = 0;
        }

        

    }
}
