using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USProApplication.Utils
{
    public class DelegateCommand<T> : CommandBase<T>
    {
        private Action<T> _execute;

        public DelegateCommand(Action<T> execute, Func<T, bool>? canExecute)
            : base(canExecute)
        {
            _execute = execute;
        }

        public DelegateCommand(Action<T> execute)
            : this(execute, null)
        { }

        public override void Execute(T parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}
