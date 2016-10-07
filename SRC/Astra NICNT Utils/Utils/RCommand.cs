using System;
using System.Windows.Input;

namespace Astra_NICNT_Utils.Utils
{
    
    /// <summary> CanExeciting enabler here for auto-binding button IsEnabled </summary>
    public class RCommand : ICommand
    {

        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;


        public RCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("Null in action");

            _execute = execute;
            _canExecute = canExecute;
        }


        public bool CanExecute(object parameter)
        {
            //return true;
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

//#pragma warning disable 67
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
//#pragma warning restore 67

    }
}
