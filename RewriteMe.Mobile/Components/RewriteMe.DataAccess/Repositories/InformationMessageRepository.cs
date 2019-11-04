using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.DataAccess.DataAdapters;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.WebApi.Models;
using SQLiteNetExtensions.Extensions;

namespace RewriteMe.DataAccess.Repositories
{
    public class InformationMessageRepository : IInformationMessageRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public InformationMessageRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<IEnumerable<InformationMessage>> GetAllAsync(DateTime minimumDateTime)
        {
            var entities = await _contextProvider.Context
                .GetAllWithChildrenAsync<InformationMessageEntity>(x => x.DatePublishedUtc >= minimumDateTime)
                .ConfigureAwait(false);

            return entities?.Select(x => x.ToInformationMessage());
        }

        public async Task DeleteAsync(DateTime minimumDateTime)
        {
            var entities = await _contextProvider.Context.InformationMessages
                .Where(x => x.DatePublishedUtc < minimumDateTime)
                .ToListAsync()
                .ConfigureAwait(false);

            if (entities.Any())
            {
                var ids = entities.Select(x => (object)x.Id);
                await _contextProvider.Context.DeleteAllIdsAsync<InformationMessageEntity>(ids).ConfigureAwait(false);
            }
        }

        public async Task InsertOrReplaceAllAsync(IEnumerable<InformationMessage> informationMessages)
        {
            var informationMessageEntities = informationMessages.Select(x => x.ToInformationMessageEntity()).ToList();
            if (!informationMessageEntities.Any())
                return;

            var existingEntities = await _contextProvider.Context.GetAllWithChildrenAsync<InformationMessageEntity>(x => true).ConfigureAwait(false);
            var mergedFileItems = existingEntities.Where(x => informationMessageEntities.All(e => e.Id != x.Id)).ToList();
            mergedFileItems.AddRange(informationMessageEntities);

            await _contextProvider.Context.RunInTransactionAsync(database =>
            {
                database.DeleteAll<InformationMessageEntity>();
                database.DeleteAll<LanguageVersionEntity>();
                database.InsertAllWithChildren(mergedFileItems);
            }).ConfigureAwait(false);
        }

        public async Task<bool> HasUnopenedMessagesAsync(DateTime minimumDateTime)
        {
            var informationMessageEntity = await _contextProvider.Context
                .InformationMessages
                .CountAsync(x => !x.WasOpened && x.DatePublishedUtc >= minimumDateTime)
                .ConfigureAwait(false);

            return informationMessageEntity > 0;
        }

        public async Task UpdateAsync(InformationMessage informationMessage)
        {
            await _contextProvider.Context.UpdateAsync(informationMessage.ToInformationMessageEntity()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<InformationMessage>> GetPendingAsync()
        {
            var entities = await _contextProvider.Context.GetAllWithChildrenAsync<InformationMessageEntity>(x => x.IsPendingSynchronization).ConfigureAwait(false);

            return entities.Select(x => x.ToInformationMessage());
        }

        public async Task ClearAsync()
        {
            await _contextProvider.Context.DeleteAllAsync<InformationMessageEntity>().ConfigureAwait(false);
        }
    }
}
