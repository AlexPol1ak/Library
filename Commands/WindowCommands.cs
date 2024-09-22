using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Library.Commands
{
    /// <summary>
    /// Класс дополнительных оконных команад.
    /// </summary>
    static public class WindowCommands
    {
        public static RoutedCommand Exit { get; set; }

        static WindowCommands()
        {
            Exit = new RoutedCommand("Exit", typeof(MainWindow));
            KeyGesture keyEscape = new(Key.Escape);
            Exit.InputGestures.Add(keyEscape);
        }
    }
}
