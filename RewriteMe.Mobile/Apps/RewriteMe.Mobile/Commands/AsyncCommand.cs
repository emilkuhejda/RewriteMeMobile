using System;
using System.Threading.Tasks;
using RewriteMe.Mobile.Utils;

namespace RewriteMe.Mobile.Commands
{
    public class AsyncCommand : IAsyncCommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _currentlyInExecution;

        public event EventHandler CanExecuteChanged;

        public AsyncCommand(Func<Task> execute)
            : this(execute, null)
        {
        }

        public AsyncCommand(Action execute)
            : this(execute, null)
        {
        }

        public AsyncCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public AsyncCommand(Action execute, Func<bool> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _execute = () =>
            {
                execute();
                return Task.CompletedTask;
            };

            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_currentlyInExecution)
            {
                return false;
            }

            if (_canExecute != null)
            {
                return _canExecute();
            }

            return true;
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync().ConfigureAwait(false);
        }

        public async Task ExecuteAsync()
        {
            _currentlyInExecution = true;
            ChangeCanExecute();

            var isFailedTask = false;
            await Task.Run(_execute)
                .ContinueWith(ct =>
                {
                    _currentlyInExecution = false;
                    ChangeCanExecuteOnMainThread();

                    isFailedTask = ct.IsFaulted;
                })
                .ConfigureAwait(false);

            if (isFailedTask)
            {
                GC.Collect();
            }
        }

        public void ChangeCanExecute()
        {
            OnCanExecuteChanged();
        }

        private void ChangeCanExecuteOnMainThread()
        {
            ThreadHelper.InvokeOnUiThread(OnCanExecuteChanged);
        }

        private void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
