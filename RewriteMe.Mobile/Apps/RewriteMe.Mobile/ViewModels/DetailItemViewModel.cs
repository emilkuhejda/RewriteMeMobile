using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Controls;

namespace RewriteMe.Mobile.ViewModels
{
    public abstract class DetailItemViewModel<T> : BindableBase
    {
        private IEnumerable<WordComponent> _words;
        private bool _isReloadCommandVisible;
        private string _transcript;
        private bool _isDirty;
        private bool _isHighlightEnabled;

        public event EventHandler IsDirtyChanged;

        protected DetailItemViewModel(
            PlayerViewModel playerViewModel,
            IDialogService dialogService,
            T detailItem)
        {
            PlayerViewModel = playerViewModel;
            DialogService = dialogService;
            DetailItem = detailItem;
            OperationScope = new AsyncOperationScope();

            IsHighlightEnabled = false;

            PlayCommand = new AsyncCommand(ExecutePlayCommandAsync);
            ReloadCommand = new DelegateCommand(ExecuteReloadCommand, CanExecuteReloadCommand);
        }

        protected PlayerViewModel PlayerViewModel { get; }

        protected IDialogService DialogService { get; }

        public AsyncOperationScope OperationScope { get; }

        public ICommand PlayCommand { get; }

        public ICommand ReloadCommand { get; }

        public bool IsHighlightEnabled
        {
            get => _isHighlightEnabled;
            set => SetProperty(ref _isHighlightEnabled, value);
        }

        public IEnumerable<WordComponent> Words
        {
            get => _words;
            set => SetProperty(ref _words, value);
        }

        public bool IsReloadCommandVisible
        {
            get => _isReloadCommandVisible;
            set => SetProperty(ref _isReloadCommandVisible, value);
        }

        public T DetailItem { get; }

        public string Time { get; protected set; }

        public string Accuracy { get; protected set; }

        public string Transcript
        {
            get => _transcript;
            set
            {
                if (SetProperty(ref _transcript, value))
                {
                    OnTranscriptChanged(value);
                    IsReloadCommandVisible = CanExecuteReloadCommand();
                    IsDirty = true;
                }
            }
        }

        public bool IsDirty
        {
            get => _isDirty;
            private set
            {
                if (SetProperty(ref _isDirty, value))
                {
                    OnIsDirtyChanged();
                }
            }
        }

        protected abstract void OnTranscriptChanged(string transcript);

        protected abstract bool CanExecuteReloadCommand();

        protected abstract Task ExecutePlayCommandAsync();

        protected abstract void ExecuteReloadCommand();

        protected void SetTranscript(string transcript)
        {
            _transcript = transcript;
        }

        private void OnIsDirtyChanged()
        {
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
