namespace USProApplication.Utils
{
    public class AsyncCommandGeneric<T>(Func<T, Task> executeAsync, Func<T, bool>? canExecute) : CommandBase<T>(canExecute), IAsyncCommand<T>
    {
        private readonly Func<T, Task> _executeAsync = executeAsync;
        private Task _executeTask = Task.CompletedTask;

        private bool _isExecuting = false;
        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                if (_isExecuting != value)
                {
                    _isExecuting = value;
                    RaiseCanExecuteChanged();
                }
            }
        }

        public AsyncCommandGeneric(Func<T, Task> executeAsync)
            : this(executeAsync, null)
        { }

        public override bool CanExecute(T parameter)
        {
            if (_isExecuting)
                return false;
            return base.CanExecute(parameter);
        }

        public override void Execute(T parameter)
        {
            ExecuteAsync(parameter);
        }

        public Task ExecuteAsync(T parameter)
        {
            if (_executeAsync == null)
                return Task.CompletedTask;

            _isExecuting = true;
            _executeTask = _executeAsync(parameter).ContinueWith((task) =>
            {
                _isExecuting = false;
                if (task.IsFaulted)
                {
                    throw task.Exception!.InnerException!;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            return _executeTask;
        }
    }
}
