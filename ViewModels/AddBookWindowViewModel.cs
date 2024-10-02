using Library.Business.Managers;
using System;
using System.Collections.Generic;
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

        public AddBookWindowViewModel(AuthorManager authorManager, BookManager bookManager)
        {
            this.authorManager = authorManager;
            this.bookManager = bookManager;
        }

    }
}
