using System;
using System.Threading.Tasks;
using Prism.Mvvm;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Mobile.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private readonly IInternalValueService _internalValueService;
        private bool _isHighlightingEnabled;

        public event EventHandler SettingsChanged;

        public SettingsViewModel(IInternalValueService internalValueService)
        {
            _internalValueService = internalValueService;
        }

        public async Task InitializeAsync()
        {
            _isHighlightingEnabled = await _internalValueService.GetValueAsync(InternalValues.IsHighlightingEnabled).ConfigureAwait(false);

            RaisePropertyChanged(nameof(IsHighlightingEnabled));
        }

        public bool IsHighlightingEnabled
        {
            get => _isHighlightingEnabled;
            set
            {
                if (SetProperty(ref _isHighlightingEnabled, value))
                {
                    AsyncHelper.RunSync(() => _internalValueService.UpdateValueAsync(InternalValues.IsHighlightingEnabled, value));

                    OnSettingsChanged();
                }
            }
        }

        private void OnSettingsChanged()
        {
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
