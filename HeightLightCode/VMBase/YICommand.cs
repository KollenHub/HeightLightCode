using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

/// <summary>
/// ViewModel绑定事件
/// </summary>
namespace HeightLightCode.VMBase
{
    public class YICommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (CanExecuteFunc != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (CanExecuteFunc != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }
        public YICommand(Action<object> action, Func<object, bool> canExcute = null)
        {
            CanExecuteFunc = canExcute ?? (x => true);
            ExecuteAction = action;
        }
        public void SetAction(Action<object> action)
        {
            ExecuteAction = action;
        }
        public void SetCanExcute(Func<object, bool> canExcute = null)
        {
            CanExecuteFunc = canExcute ?? (x => true);
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc== null ? true : CanExecuteFunc(parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteAction?.Invoke(parameter);
        }
        public Func<object, bool> CanExecuteFunc { get; private set; }
        public Action<object> ExecuteAction { get; private set; }
    }
}
