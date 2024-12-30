namespace USProApplication.Utils;

public interface IAsyncCommand<T> : ICommand<T>
{
    Task ExecuteAsync(T parameter);
}
