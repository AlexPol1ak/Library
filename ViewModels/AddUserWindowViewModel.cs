using Library.Business.Managers;
using Library.Commands;
using Library.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Library.ViewModels
{
    public class AddUserWindowViewModel : ViewModelBase, IDataErrorInfo
    {
        public AddUserWindowViewModel(UserManager userManager)
        {
            this.userManager = userManager;
            getAllEmail();
        }

        private UserManager userManager;
        public EventHandler<EventArgs> EndWork;
        public User? NewUser { get; private set; } = null;
        public bool? DialogResult { get; private set; } = null;
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set { Set(ref _title, value); }
        }

        private List<string> allEmail = new();
        

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set
            {
                Set(ref _firstName, value);
            }

        }
        public bool FirstNameHasError { get; private set; } = true;

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set
            {
                Set(ref _lastName, value);
            }
        }
        public bool LastNameHasError { get; private set; } = true;

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { Set(ref _email, value); }
        }
        public bool EmailHasError { get; private set; } = true;

        private string? _patronymic = null;
        public string? Patronymic
        {
            get => _patronymic;
            set { Set(ref _patronymic, value); }
        }
        public bool PatronymicHasError { get; private set; } = false;

        private ICommand _userSaveCmd;
        private ICommand _userCancelCmd;

        public ICommand UserSaveCmd => _userSaveCmd ??=
            new RelayCommand(userSaveExecuted, canUserSave);

        /// <summary>
        /// Проверяет корректность введенных данных и определяет возможность сохранения.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool canUserSave(object arg)
        {
            if(FirstNameHasError || LastNameHasError || PatronymicHasError || EmailHasError)
                return false;
            
            return true;
        }

        /// <summary>
        /// Создает нового читателя и завершает работу окна.
        /// </summary>
        /// <param name="obj"></param>
        private void userSaveExecuted(object obj)
        {
            User user = new User(Email, FirstName, LastName, Patronymic);
            userManager.CreateUser(user);
            userManager.SaveChanges();
            DialogResult = true;
            NewUser = user;
            EndWork?.Invoke(this, EventArgs.Empty);
        }

        public ICommand UserCancelCmd => _userCancelCmd ??=
            new RelayCommand(userCancelExecuted);

        /// <summary>
        /// Завершает работу окна.
        /// </summary>
        /// <param name="obj"></param>
        private void userCancelExecuted(object obj)
        {
            DialogResult = false;
            NewUser = null;
            EndWork?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Получает email всех читателей.
        /// </summary>
        private void getAllEmail()
        {
            foreach (User user in userManager.GetUsers()) allEmail.Add(user.Email);
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                switch (columnName)
                {
                    case nameof(FirstName):
                        {
                            if(string.IsNullOrEmpty(FirstName) || string.IsNullOrWhiteSpace(FirstName))
                            {
                                FirstNameHasError = true;
                                error = "Имя не может быть пустым!";
                            }
                            else
                            {
                                if(FirstName.Length < 2 || FirstName.Length > 45)
                                {
                                    FirstNameHasError = true;
                                    error = "Неверная длинна!";
                                }
                                else
                                {
                                    FirstNameHasError = false;
                                }
                            }
                             
                        }
                        break;
                    case nameof(LastName):
                        if (string.IsNullOrEmpty(LastName) || string.IsNullOrWhiteSpace(LastName))
                        {
                            LastNameHasError = true;
                            error = "Имя не может быть пустым!";
                        }
                        else
                        {
                            if (LastName.Length < 2 || LastName.Length > 45)
                            {
                                LastNameHasError = true;
                                error = "Неверная длинна!";
                            }
                            else
                            {
                                LastNameHasError = false;
                            }
                        }
                        break;
                    case nameof(Patronymic):
                        break;
                    case nameof(Email):
                        {
                            if(string.IsNullOrEmpty(Email) || string.IsNullOrWhiteSpace(Email))
                            {
                                EmailHasError = true;
                                error = "Поле Email не может быть пустым!";
                            }
                            else
                            {
                                var emailAttribute = new EmailAddressAttribute();
                                if(emailAttribute.IsValid(Email) != true)
                                {
                                    EmailHasError = true;
                                    error = "Некорректный Email!";
                                }
                                else
                                {
                                    if (allEmail.Contains(Email))
                                    {
                                        EmailHasError = true;
                                        error = "Такой email уже зарегистрирован!";
                                    }
                                    else
                                    {
                                        EmailHasError = false;
                                    }
                                }
                            }
                                                      
                        }
                        break;
                }

                return error;
            }
        }
    }
}

