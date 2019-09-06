using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IInformationMessageRepository
    {
        Task<IEnumerable<InformationMessage>> GetAllAsync(DateTime minimumDateTime);

        Task InsertOrReplaceAllAsync(IEnumerable<InformationMessage> informationMessages);

        Task<bool> IsUnopenedMessageAsync();

        Task UpdateAsync(InformationMessage informationMessage);

        Task ClearAsync();
    }
}
