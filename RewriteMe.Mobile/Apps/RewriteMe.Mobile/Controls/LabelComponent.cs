using System;
using Prism.Mvvm;

namespace RewriteMe.Mobile.Controls
{
    public class LabelComponent : BindableBase
    {
        private bool _isHighlighted;

        public string Text { get; set; }

        public TimeSpan StartTime { get; set; }

        public bool IsHighlighted
        {
            get => _isHighlighted;
            set => SetProperty(ref _isHighlighted, value);
        }
    }
}
