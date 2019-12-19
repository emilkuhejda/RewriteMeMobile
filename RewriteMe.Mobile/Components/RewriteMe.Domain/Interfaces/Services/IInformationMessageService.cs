using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IInformationMessageService
    {
        event EventHandler MessageOpened;

        Task SynchronizationAsync(DateTime applicationUpdateDate);

        Task<IEnumerable<InformationMessage>> GetAllForLastWeekAsync();

        Task<bool> HasUnopenedMessagesForLastWeekAsync();

        Task MarkAsOpenedAsync(InformationMessage informationMessage);

        Task SendPendingAsync();
    }
}
