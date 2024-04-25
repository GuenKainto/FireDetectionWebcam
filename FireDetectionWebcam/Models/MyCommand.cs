using System;
using System.Windows.Input;

namespace FireDetectionWebcam.Models
{
    public class MyCommand : ICommand
    {
        private Action<object> _TargetExecuteMethod;
        private Func<bool> _TargetCanExecuteMethod;

        public MyCommand(Action<object> executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
            
        }

        public MyCommand(Action<object> executeMethod, Func<bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }



        public void RaiseCanExecuteChanged()
        {
            if (this != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return _TargetCanExecuteMethod != null ? _TargetCanExecuteMethod() : _TargetExecuteMethod != null;
        }

        // Beware - should use weak references if command instance lifetime
        //  is longer than lifetime of UI objects that get hooked up to command

        // Prism commands solve this in their implementation
        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            _TargetExecuteMethod(parameter);
        }
    }
}
