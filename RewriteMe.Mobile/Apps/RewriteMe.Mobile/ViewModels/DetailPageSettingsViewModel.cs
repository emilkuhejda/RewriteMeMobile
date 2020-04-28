using System.Threading.Tasks;
using Prism.Mvvm;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Mobile.ViewModels
{
    public class DetailPageSettingsViewModel : BindableBase
    {
        private readonly IInternalValueService _internalValueService;
        private bool _isHighlightingEnabled;

        public DetailPageSettingsViewModel(IInternalValueService internalValueService)
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
                }
            }
        }
    }
}
