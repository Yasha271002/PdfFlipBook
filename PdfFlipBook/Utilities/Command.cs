using System;
using System.Windows.Input;

namespace PdfFlipBook.Utilities
{
    public class Command : ICommand
    {
        public Command( Action execute)
        {
            _execute = execute;
        }

        public Command( Action<object> execute)
        {
            _executeP = execute;
        }

        public Command( Action<object> execute,  Predicate<object> canExecute)
        {
            _executeP = execute;
            _canExecute = canExecute;
        }

        private readonly Action _execute;
        private readonly Action<object> _executeP;
        private readonly Predicate<object> _canExecute;
        
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke();
            _executeP?.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
