﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Library.ViewModels
{
    /// <summary>
    /// Базовый класс ViewModel
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        protected bool Set<T>(ref T prop, T value, [CallerMemberName] string propName = null)
        {
            if (Equals(prop, value)) return false;

            prop = value;
            OnPropertyChanged(propName);
            return true;
        }
    }
}
