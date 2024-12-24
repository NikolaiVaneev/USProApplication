using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace USProApplication.Utils
{
    public abstract class CommandBase<T> : ICommand<T>
    {
        private Func<T, bool>? _canExecute;

        public CommandBase(Func<T, bool>? canExecute = null)
        {
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public virtual bool CanExecute(T parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public bool CanExecute(object? parameter)
        {
            return CanExecute(GetGenericParameter(parameter, suppressCastException: true));
        }

        public abstract void Execute(T parameter);

        public void Execute(object? parameter)
        {
            Execute(GetGenericParameter(parameter));
        }

        internal static T GetGenericParameter(object? parameter, bool suppressCastException = false)
        {
            parameter = TryCast(parameter, typeof(T));
            if (parameter == null || parameter is T)
            {
                return (T)parameter!;
            }

            if (suppressCastException)
            {
                return default!;
            }

            throw new InvalidCastException($"CommandParameter: Unable to cast object of type '{parameter.GetType().FullName}' to type '{typeof(T).FullName}'");
        }

        public static object? TryCast(object? value, Type targetType)
        {
            Type type = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (type.IsEnum && value is string)
            {
                value = Enum.Parse(type, (string)value, ignoreCase: false);
            }
            else if (value is IConvertible && !targetType.IsAssignableFrom(value.GetType()))
            {
                value = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            if (value == null && targetType.IsValueType)
            {
                value = Activator.CreateInstance(targetType);
            }
            return value;
        }
    }
}