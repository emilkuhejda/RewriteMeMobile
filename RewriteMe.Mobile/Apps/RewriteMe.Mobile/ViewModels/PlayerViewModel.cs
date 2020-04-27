using System;
using System.IO;
using System.Windows.Input;
using Plugin.SimpleAudioPlayer;
using Prism.Commands;
using Prism.Mvvm;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class PlayerViewModel : BindableBase, IDisposable
    {
        private ISimpleAudioPlayer _player;
        private bool _isVisible;
        private bool _isPlaying;
        private double _audioCurrentProgress;
        private TimeSpan _totalTime;
        private double _duration = 59;
        private string _position;

        private bool _disposed;

        public event EventHandler Tick;

        public PlayerViewModel()
        {
            _player = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            _player.PlaybackEnded += HandlePlaybackEnded;

            StartPauseCommand = new DelegateCommand(ExecuteStartPauseCommand);
        }

        public ICommand StartPauseCommand { get; }

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => SetProperty(ref _isPlaying, value);
        }

        public double AudioCurrentProgress
        {
            get => _audioCurrentProgress;
            set
            {
                if (SetProperty(ref _audioCurrentProgress, value))
                {
                    if (_player != null && _player.IsPlaying)
                    {
                        _player.Seek(value);
                    }
                }
            }
        }

        public TimeSpan TotalTime
        {
            get => _totalTime;
            set => SetProperty(ref _totalTime, value);
        }

        public double Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        public string Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        public double CurrentPosition => _player.CurrentPosition;

        public void Load(byte[] data)
        {
            if (_player == null)
                return;

            if (_player.IsPlaying)
                _player.Stop();

            if (data == null)
                return;

            using (var memoryStream = new MemoryStream(data))
            {
                _player.Load(memoryStream);
            }

            Duration = _player.Duration;
            TotalTime = TimeSpan.FromSeconds(Duration);
            IsVisible = true;
            IsPlaying = _player.IsPlaying;
        }

        public void Play()
        {
            if (_player == null)
                throw new InvalidOperationException("Recorder is not loaded");

            Device.StartTimer(TimeSpan.FromSeconds(0.5), UpdatePosition);

            _player.Play();

            IsPlaying = _player.IsPlaying;
        }

        public void Pause()
        {
            if (_player == null)
                return;

            if (!_player.IsPlaying)
                return;

            _player.Pause();

            IsPlaying = _player.IsPlaying;
        }

        private bool UpdatePosition()
        {
            if (_player == null)
                return false;

            var currentPosition = TimeSpan.FromSeconds((int)_player.CurrentPosition);

            Position = $"{currentPosition:mm\\:ss} / {TotalTime:mm\\:ss}";

            _audioCurrentProgress = _player.CurrentPosition;
            RaisePropertyChanged(nameof(AudioCurrentProgress));

            OnTick();

            return _player.IsPlaying;
        }

        private void ExecuteStartPauseCommand()
        {
            if (_player.IsPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        private void HandlePlaybackEnded(object sender, EventArgs e)
        {
            IsPlaying = _player.IsPlaying;
        }

        private void OnTick()
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_player != null)
                {
                    _player.Stop();
                    _player.PlaybackEnded -= HandlePlaybackEnded;
                    _player.Dispose();
                    _player = null;
                }
            }

            _disposed = true;
        }
    }
}
