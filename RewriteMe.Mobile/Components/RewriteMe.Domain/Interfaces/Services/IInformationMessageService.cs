using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IInformationMessageService
    {
        Task SynchronizationAsync(DateTime applicationUpdateDate);

        Task<IEnumerable<InformationMessage>> GetAllForLastWeekAsync();

        Task<bool> IsUnopenedMessageAsync();

        Task MarkAsOpenedAsync(InformationMessage informationMessage);

        Task SendPendingAsync();
    }
}
