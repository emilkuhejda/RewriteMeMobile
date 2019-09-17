using System;
using System.ComponentModel;
using RewriteMe.Domain.Interfaces.Required;

namespace RewriteMe.Mobile.Localization
{
    public class TranslationData : INotifyPropertyChanged, IDisposable
    {
        private readonly string _key;
        private readonly ILocalizer _localizer;
        private readonly Func<string, string> _translateAction;

        public event PropertyChangedEventHandler PropertyChanged;

        public TranslationData(string key, ILocalizer localizer, Func<string, string> translateAction)
        {
            _key = key;
            _localizer = localizer;
            _translateAction = translateAction;

            _localizer.CultureInfoChanged += OnLocaleChanged;
        }

        ~TranslationData()
        {
            Dispose(false);
        }

        public object Value => _translateAction(_key);

        private void OnLocaleChanged(object sender, EventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _localizer.CultureInfoChanged -= OnLocaleChanged;
            }
        }
    }
}
