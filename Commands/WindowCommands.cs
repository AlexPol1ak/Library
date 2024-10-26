using System.Windows.Input;

namespace Library.Commands
{
    /// <summary>
    /// Класс дополнительных оконных команд.
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
