using System;
using System.Windows.Input;
using Prism.Commands;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Utils
{
    public class DeveloperMode : BindableObject
    {
        private const int UnlockCounterMax = 3;

        private DateTime? _previousClickDateTime;
        private int _unlockCounter;
        private bool _unlocked;

        public event EventHandler UnlockedEvent;

        public DeveloperMode()
        {
            UnlockCommand = new DelegateCommand(ExecuteUnlockCommand, CanExecuteUnlockCommand);
        }

        public bool IsEnabled { get; set; }

        public bool Unlocked
        {
            get => _unlocked;
            set
            {
                _unlocked = value;
                OnPropertyChanged(nameof(Unlocked));
            }
        }

        public ICommand UnlockCommand { get; }

        private bool CanExecuteUnlockCommand()
        {
            return IsEnabled && !Unlocked;
        }

        private void ExecuteUnlockCommand()
        {
            UnlockDeveloperMode();
        }

        private void UnlockDeveloperMode()
        {
            if (!_previousClickDateTime.HasValue)
                _previousClickDateTime = DateTime.Now;

            var actualClickDateTime = DateTime.Now;
            if (actualClickDateTime.AddSeconds(-5) < _previousClickDateTime)
            {
                if (_unlockCounter <= UnlockCounterMax)
                {
                    _unlockCounter++;
                }
                else
                {
                    Unlocked = true;
                    OnUnlockedEvent();
                }
            }
            else
            {
                _unlockCounter = 0;
                _previousClickDateTime = null;
            }
        }

        private void OnUnlockedEvent()
        {
            UnlockedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
