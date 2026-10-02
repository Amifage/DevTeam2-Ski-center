using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Presentationslager.Command
{
    public class RelayCommand : ICommand //Den ska heta Relay för att inte blandas ihop med namespacet. Funkar som en länk mellan knappar.
    {

        private readonly Action<object> _execute; //En pekare, avgör vilken metod som ska köras när man trycker på knappen.
        private readonly Predicate<object> _canExecute; //Kontrollfunktion som avgör om knappen ska vara aktiverad eller inte.

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
