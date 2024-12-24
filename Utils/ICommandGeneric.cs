using System.Windows.Input;

namespace USProApplication.Utils;

public interface ICommand<T> : ICommand
{
    void Execute(T param);
    bool CanExecute(T param);
}