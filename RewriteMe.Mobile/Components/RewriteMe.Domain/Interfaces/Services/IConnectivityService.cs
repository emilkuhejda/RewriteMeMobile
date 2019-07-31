using System;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IConnectivityService
    {
        event EventHandler ConnectivityChanged;

        bool IsConnected { get; }
    }
}
