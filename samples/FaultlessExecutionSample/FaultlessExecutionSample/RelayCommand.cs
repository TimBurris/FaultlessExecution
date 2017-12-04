using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaultlessExecutionSample
{
    /// <summary>
    /// A command whose sole purpose is to 
    /// relay its functionality to other
    /// objects by invoking delegates. The
    /// default return value for the CanExecute
    /// method is 'true'.
    /// </summary>
    public class RelayCommand<T> : System.Windows.Input.ICommand
    {
        public RelayCommand(Action<T> execute) : this(execute, null) { }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute) : this(execute, canExecute, label: "") { }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute, string label)
        {
            _Execute = execute;
            _CanExecute = canExecute;

            Label = label;
        }

        readonly Action<T> _Execute = null;
        readonly Predicate<T> _CanExecute = null;

        public event EventHandler CanExecuteChanged;

        public string Label { get; set; }

        public void Execute(object parameter)
        {
            _Execute((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            //they can execute if it has not been forcefully disabled and their canexecute code says yes
            return (_CanExecute == null ? true : _CanExecute((T)parameter));
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// provides a generic object implementation of <see cref="RelayCommand&lt;T&gt;"/>
    /// </summary>
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute) : this(execute, canExecute, "") { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute, string label) : base(execute, canExecute, label) { }
    }
}
