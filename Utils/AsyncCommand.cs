namespace USProApplication.Utils
{
    public class AsyncCommand : AsyncCommandGeneric<object>
    {
        public AsyncCommand(Func<object, Task> executeAsync, Func<object, bool>? canExecute)
            : base(executeAsync, canExecute)
        { }

        public AsyncCommand(Func<object?, Task> executeAsync)
            : base(executeAsync)
        { }

        public AsyncCommand(Func<Task> executeAsync, Func<bool>? canExecute)
            : this(executeAsync != null ? (object param) => executeAsync() : null,
                  canExecute != null ? (object param) => canExecute() : null)
        { }

        public AsyncCommand(Func<Task> executeAsync)
            : this(executeAsync, null)
        { }
    }

}
