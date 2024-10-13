using Library.Business.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.ViewModels
{
    public class AddUserWindowViewModel: ViewModelBase
    {
        public AddUserWindowViewModel(UserManager userManager)
        {
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set { Set(ref _title, value); }
        }
    
    }
}
