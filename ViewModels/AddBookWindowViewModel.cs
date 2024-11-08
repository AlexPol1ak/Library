﻿using Library.Business.Managers;
using Library.Commands;
using Library.Domain.Entities.Books;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Library.ViewModels
{
    /// <summary>
    /// ViewModel окна добавления книги.
    /// </summary>
    public class AddBookWindowViewModel : ViewModelBase, IDataErrorInfo
    {
        public event EventHandler EndWork;
        public bool? DialogResult { get; private set; } = null;
        public Book? NewBook { get; private set; } = null;

        private AuthorManager authorManager;
        private BookManager bookManager;
        private GenreManager genreManager;
        private TermManager termManager;
        private RackManager rackManager;

        public AddBookWindowViewModel(AuthorManager authorManager, BookManager bookManager,
            GenreManager genreManager, TermManager termManager, RackManager rackManager)
        {
            this.authorManager = authorManager;
            this.bookManager = bookManager;
            this.genreManager = genreManager;
            this.termManager = termManager;
            this.rackManager = rackManager;

            initData();
        }

        /// <summary>
        /// Инициализация элементов окна.
        /// </summary>
        private void initData()
        {
            // Инициализация жанров
            Genres = genreManager.GetGenres().ToList();
            if (Genres.Count > 0) SelectedIndexGenre = 0;

            // Инициализация авторов
            _allAuthors = authorManager.GetAuthors().OrderBy(a => a.ShortName).ToList();
            AvailableAuthors = new();
            SelectedAuthors = new();
            foreach (Author author in _allAuthors) AvailableAuthors.Add(author);
            if (AvailableAuthors.Count > 0) SelectedIndexAvailableAuthors = 0;

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
        private int _selectedIndexGenre;
        public int SelectedIndexGenre
        {
            get { return _selectedIndexGenre; }
            set { Set(ref _selectedIndexGenre, value); }
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
            set { Set(ref _selectedIndexSelectedAuthors, value); }
        }
        // Выбор из списка доступных авторов
        public int SelectedIndexAvailableAuthors
        {
            get { return _selectedIndexAvailableAuthors; }
            set { Set(ref _selectedIndexAvailableAuthors, value); }
        }

        #endregion

        #region Commands       
        //Команда сохранения новой книги
        private ICommand _saveBookCmd;
        public ICommand SaveBookCmd => _saveBookCmd ??=
            new RelayCommand(saveBookExecuted, canSaveBook);

        /// <summary>
        /// Метод проверки возможности сохранения новой книги.
        /// Проверяет ошибки флаги валидации
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool canSaveBook(object arg)
        {
            if (
                BookNameHasError || NumberPagesHasError || PublicationYearHasError ||
                DescriptionHasError || SelectedAuthorsHasError
                ) return false;
            return true;
        }

        /// <summary>
        /// Обрабочтик для команды сохранения книги.
        /// Сохраняет книгу в базе данных.
        /// </summary>
        /// <param name="id"></param>
        private void saveBookExecuted(object id)
        {
            Book book = new Book(Name, NumberPages, PublicationYear, Description);
            book.Authors = SelectedAuthors;
            book.Genre = Genres[SelectedIndexGenre];
            // Инициализация правила выдачи
            if (book.NumberPages < 200)
            {
                Term term = termManager.FindTerm(f => f.MaximumDays != null).First();
                book.Term = term;
            }
            else
            {
                Term term = termManager.FindTerm(f => f.MaximumDays == null).First();
                book.Term = term;
            }
            // Инициализация стеллажа хранения.
            IEnumerable<Rack> racks = rackManager.GetRacks("Books").
                Where(r => r.Name.Contains(book.Genre.Name)).
                OrderBy(r => r.Books.Count);
            Rack rack = racks.ToList()[0];
            book.Rack = rack;

            bookManager.CreateBook(book);
            bookManager.SaveChanges();
            DialogResult = true;
            NewBook = book;

            EndWork?.Invoke(this, new EventArgs());
        }

        // Команда закрытия окна
        private ICommand _cancelCmd;
        public ICommand CancelCmd => _cancelCmd ??=
            new RelayCommand((id) => EndWork?.Invoke(this, new EventArgs()));

        // Команда добавления автора к добавляемой книге
        private ICommand _addAuthorCmd;
        public ICommand AddAuthorCmd => _addAuthorCmd ??=
            new RelayCommand(addAuthorExecuted,
                (id) => AvailableAuthors.Count > 0 && SelectedIndexAvailableAuthors <= AvailableAuthors.Count - 1
                );

        /// <summary>
        /// Обработчик для команды добавления автора
        /// Перемещает книгу из коллекции допступных авторов в коллекцию выбранных авторов.
        /// </summary>
        /// <param name="obj"></param>
        private void addAuthorExecuted(object obj)
        {
            // Получить выбранного автора из коллекции доступных авторов.
            Author author = AvailableAuthors[SelectedIndexAvailableAuthors];
            // Добавить в коллекцию выбранных авторов.
            SelectedAuthors.Add(author);
            // Установить индекс в выбора в ListView коллекции выбранных авторов на добавленого автора
            SelectedIndexSelectedAuthors = SelectedAuthors.IndexOf(author);
            // Удалить автора из коллекции доступных авторов
            AvailableAuthors.Remove(author);
            // Сбросить индекс выбора доступных авторов.
            SelectedIndexAvailableAuthors = 0;
        }

        // Команда удаления автора из ListView добавляймых авторов
        private ICommand _removeAuthorCmd;
        public ICommand RemoveAuthorCmd => _removeAuthorCmd ??=
            new RelayCommand(removeAuthorExecuted,
                (id) => SelectedAuthors.Count > 0 && SelectedIndexSelectedAuthors <= SelectedAuthors.Count - 1
                );

        /// <summary>
        /// Обработчик для команды удаления автора.
        /// Перемещает автора из коллекции выбранных авторов в коллекцию доступных авторов.
        /// </summary>
        /// <param name="obj"></param>
        private void removeAuthorExecuted(object obj)
        {
            // Получить удаляемого автора из коллекции выбранных авторов
            Author author = SelectedAuthors[SelectedIndexSelectedAuthors];
            // Добавить в ComboBox коллекцию доступных авторов
            AvailableAuthors.Add(author);
            // Удалить из коллекции выбранных авторов
            SelectedAuthors.Remove(author);
            // Сбросить индекс в коллекции ListView выбранных авторов
            SelectedIndexSelectedAuthors = 0;
            // Обвновление и сортировка коллекции доступных авторов
            List<Author> TempAuthors = new List<Author>(AvailableAuthors).OrderBy(a => a.ShortName).ToList();
            AvailableAuthors.Clear();
            foreach (Author a in TempAuthors) AvailableAuthors.Add(a);
            // Установка индекса выбора на перемещенного автора.
            SelectedIndexAvailableAuthors = AvailableAuthors.IndexOf(author);
        }

        #endregion

        #region Validation
        public bool BookNameHasError { get; private set; } = false;
        public bool NumberPagesHasError { get; private set; } = false;
        public bool PublicationYearHasError { get; private set; } = false;
        public bool DescriptionHasError { get; private set; } = false;
        public bool _selectedAuthorsHasError = false;
        public bool SelectedAuthorsHasError
        {
            get
            {
                if (SelectedAuthors.Count < 1) _selectedAuthorsHasError = true;
                else _selectedAuthorsHasError = false;
                return _selectedAuthorsHasError;
            }
            set { Set(ref _selectedAuthorsHasError, value); }
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(Name):
                        if (string.IsNullOrEmpty(Name) || string.IsNullOrWhiteSpace(Name))
                        {
                            error = "Поле не может быть пустым!";
                            BookNameHasError = true;
                        }
                        else
                        {
                            if (Name.Length < 1 || Name.Length > 45)
                            {
                                BookNameHasError = true;
                                error = "Поле должно быть не короче 1 и не длиннее 45 символов!";
                            }
                            else
                            {
                                BookNameHasError = false;
                            }
                        }
                        break;
                    case nameof(NumberPages):
                        {
                            if (NumberPages < 1 || NumberPages > 9999)
                            {
                                error = "Количество страниц может быть от 1 до 9999";
                                NumberPagesHasError = true;
                            }
                            else NumberPagesHasError = false;
                            break;
                        }
                    case nameof(PublicationYear):
                        {
                            int maxYear = DateTime.Now.Year;
                            if (PublicationYear < 1900 || PublicationYear > maxYear)
                            {
                                error = $"Дата публикации должна быть от 1900 до {maxYear} г.";
                                PublicationYearHasError = true;
                            }
                            else PublicationYearHasError = false;
                            break;
                        }
                    case nameof(Description):
                        {
                            if (Description != null && Description.Length > 500)
                            {
                                error = $"Описание не должно превышать 500 символов";
                                DescriptionHasError = true;
                            }
                            else DescriptionHasError = false;
                            break;
                        }
                }
                return error;
            }
        }

        #endregion

    }
}
