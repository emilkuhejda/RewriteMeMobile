using System;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Services
{
    public class ConnectivityService : IConnectivityService
    {
        private readonly IConnectivity _connectivity;

        public event EventHandler ConnectivityChanged;

        public ConnectivityService(IConnectivity connectivity)
        {
            _connectivity = connectivity;

            connectivity.ConnectivityChanged += HandleConnectivityChanged;
        }

        public bool IsConnected
        {
            get
            {
                if (!CrossConnectivity.IsSupported)
                    return true;

                return _connectivity.IsConnected;
            }
        }

        private void HandleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            OnConnectivityChanged();
        }

        private void OnConnectivityChanged()
        {
            ConnectivityChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
