using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ISubscriptionProductService
    {
        Task SynchronizationAsync(DateTime applicationUpdateDate);

        Task<IEnumerable<SubscriptionProduct>> GetAsync();
    }
}
