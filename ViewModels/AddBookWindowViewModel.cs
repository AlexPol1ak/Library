using Library.Business.Managers;
using Library.Domain.Entities.Books;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Library.ViewModels
{
    public class AddBookWindowViewModel : ViewModelBase
    {
        private AuthorManager authorManager;
        private BookManager bookManager;
        private GenreManager genreManager;

        public AddBookWindowViewModel(AuthorManager authorManager, BookManager bookManager,
            GenreManager genreManager)
        {
            this.authorManager = authorManager;
            this.bookManager = bookManager;
            this.genreManager = genreManager;

            initData();
        }

        private void initData()
        {
            // Инициализация жанров
            Genres = genreManager.GetGenres().ToList();
            if (Genres.Count > 0) SelectedIndex = 0;

            // Инициализация авторов
            _allAuthors = authorManager.GetAuthors().OrderBy(a=>a.ShortName).ToList();
            AvailableAuthors = new();
            SelectedAuthors = new();
            foreach(Author author in _allAuthors) AvailableAuthors.Add(author);
            if(AvailableAuthors.Count > 0) SelectedIndexAvailableAuthors = 0;
                
        }
        #region Data
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }                       
        }

        private int _numberPages = 1;
        public int NumberPages
        {
            get { return _numberPages; }
            set { Set(ref _numberPages, value); }
        }

        private int _publicationYear = 1900;
        public int PublicationYear
        {
            get { return _publicationYear; }
            set { Set(ref _publicationYear, value); }
        }

        private string? _description = null;
        public string? Description
        {
            get { return _description; }
            set
            {
                Set(ref _description, value);
            }
        }

        private List<Genre> _genres;
        public List<Genre> Genres
        {
            get { return _genres; }
            private set { Set(ref _genres, value); }
        }
        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set {Set(ref _selectedIndex, value); }
        }
       
        private List<Author> _allAuthors;
        private ObservableCollection<Author> _availableAuthors;
        private ObservableCollection<Author> _selectedAuthors;
        // Авторы доступные для установки
        public ObservableCollection<Author> AvailableAuthors 
        { 
            get { return _availableAuthors; }
            private set { Set(ref _availableAuthors, value); }
        }
        // Установленные авторы
        public ObservableCollection<Author> SelectedAuthors 
        {
            get { return _selectedAuthors; }
            private set { Set(ref _selectedAuthors, value); }
        }        

        private int _selectedIndexSelectedAuthors;
        private int _selectedIndexAvailableAuthors;
        // Выбор из списка установленных авторов
        public int SelectedIndexSelectedAuthors 
        {
            get { return _selectedIndexSelectedAuthors; }
            set {Set (ref _selectedIndexSelectedAuthors, value); }
        }
        // Выбор из списка доступных авторов
        public int SelectedIndexAvailableAuthors
        {
            get { return _selectedIndexAvailableAuthors; }
            set { Set(ref _selectedIndexAvailableAuthors, value); }
        }
        #endregion

        #region Commands
        #endregion

    }
}
