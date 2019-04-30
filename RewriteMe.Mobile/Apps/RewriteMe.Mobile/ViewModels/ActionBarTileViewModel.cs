using System.Windows.Input;
using Prism.Mvvm;

namespace RewriteMe.Mobile.ViewModels
{
    public class ActionBarTileViewModel : BindableBase
    {
        private string _text;
        private bool _isEnabled = true;
        private string _fullyQualifiedIconKey;
        private string _iconKeyEnabled;
        private string _iconKeyDisabled;
        private ICommand _selectedCommand;

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                var hasChanged = SetProperty(ref _isEnabled, value);
                if (hasChanged)
                {
                    ReevaluateFullyQualifiedIconKey();
                }
            }
        }

        public string FullyQualifiedIconKey
        {
            get => _fullyQualifiedIconKey;
            private set => SetProperty(ref _fullyQualifiedIconKey, value);
        }

        public string IconKeyEnabled
        {
            get => _iconKeyEnabled;
            set
            {
                if (SetProperty(ref _iconKeyEnabled, value))
                {
                    ReevaluateFullyQualifiedIconKey();
                }
            }
        }

        public string IconKeyDisabled
        {
            get => _iconKeyDisabled;
            set
            {
                if (SetProperty(ref _iconKeyDisabled, value))
                {
                    ReevaluateFullyQualifiedIconKey();
                }
            }
        }

        public ICommand SelectedCommand
        {
            get => _selectedCommand;
            set => SetProperty(ref _selectedCommand, value);
        }

        private void ReevaluateFullyQualifiedIconKey()
        {
            if (string.IsNullOrEmpty(IconKeyEnabled))
            {
                FullyQualifiedIconKey = null;
                return;
            }

            FullyQualifiedIconKey = IsEnabled ? IconKeyEnabled : IconKeyDisabled;
        }
    }
}
