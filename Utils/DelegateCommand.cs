#nullable disable

namespace USProApplication.Utils;

public class DelegateCommand : DelegateCommand<object>
{
    public DelegateCommand(Action<object> execute, Func<object, bool>? canExecute)
        : base(execute, canExecute)
    { }

    public DelegateCommand(Action<object> execute)
        : base(execute)
    { }

    public DelegateCommand(Action execute, Func<bool> canExecute)
        : this(execute != null ? (object param) => execute() : null,
              canExecute != null ? (object param) => canExecute() : null)
    { }
    public DelegateCommand(Action execute)
        : this(execute, null)
    { }
}
