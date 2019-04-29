using System;
using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ILastUpdatesService
    {
        Task InitializeAsync();

        DateTime GetFileItemVersion();

        DateTime GetTranscribeItemVersion();
    }
}
