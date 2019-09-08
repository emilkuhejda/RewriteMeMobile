using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Services
{
    public class InformationMessageService : IInformationMessageService
    {
        private readonly IInternalValueService _internalValueService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly IInformationMessageRepository _informationMessageRepository;
        private readonly ILogger _logger;

        public InformationMessageService(
            IInternalValueService internalValueService,
            IRewriteMeWebService rewriteMeWebService,
            IInformationMessageRepository informationMessageRepository,
            ILoggerFactory loggerFactory)
        {
            _internalValueService = internalValueService;
            _rewriteMeWebService = rewriteMeWebService;
            _informationMessageRepository = informationMessageRepository;
            _logger = loggerFactory.CreateLogger(typeof(InformationMessageService));
        }

        public async Task SynchronizationAsync(DateTime applicationUpdateDate)
        {
            var lastInformationMessageSynchronizationTicks = await _internalValueService.GetValueAsync(InternalValues.InformationMessageSynchronizationTicks).ConfigureAwait(false);
            var lastInformationMessageSynchronization = new DateTime(lastInformationMessageSynchronizationTicks);
            _logger.Debug($"Update file items with timestamp '{lastInformationMessageSynchronization.ToString("d", CultureInfo.InvariantCulture)}'.");

            if (applicationUpdateDate >= lastInformationMessageSynchronization)
            {
                var httpRequestResult = await _rewriteMeWebService.GetInformationMessagesAsync(lastInformationMessageSynchronization).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    var informationMessages = httpRequestResult.Payload.ToList();
                    _logger.Info($"Web server returned {informationMessages.Count} items for synchronization.");

                    await _informationMessageRepository.InsertOrReplaceAllAsync(informationMessages).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.InformationMessageSynchronizationTicks, DateTime.UtcNow.Ticks).ConfigureAwait(false);
                }
            }

            SendPendingInformationMessagesAsync().FireAndForget();
        }

        public async Task<IEnumerable<InformationMessage>> GetAllForLastWeekAsync()
        {
            var minimumDateTime = DateTime.UtcNow.AddDays(-7);
            return await _informationMessageRepository.GetAllAsync(minimumDateTime).ConfigureAwait(false);
        }

        public async Task<bool> IsUnopenedMessageAsync()
        {
            return await _informationMessageRepository.IsUnopenedMessageAsync().ConfigureAwait(false);
        }

        public async Task MarkAsOpenedAsync(InformationMessage informationMessage)
        {
            informationMessage.WasOpened = true;

            await _informationMessageRepository.UpdateAsync(informationMessage).ConfigureAwait(false);

            if (informationMessage.IsUserSpecific)
            {
                MarkMessageAsOpenedAsync(informationMessage).FireAndForget();
            }
        }

        private async Task MarkMessageAsOpenedAsync(InformationMessage informationMessage)
        {
            var httpRequestResult = await _rewriteMeWebService.MarkMessageAsOpenedAsync(informationMessage.Id).ConfigureAwait(false);
            if (httpRequestResult.State != HttpRequestState.Success)
            {
                informationMessage.IsPendingSynchronization = true;

                await _informationMessageRepository.UpdateAsync(informationMessage).ConfigureAwait(false);
            }
        }

        private async Task SendPendingInformationMessagesAsync()
        {
            var pendingInformationMessages = await _informationMessageRepository.GetPendingAsync().ConfigureAwait(false);
            var informationMessages = pendingInformationMessages.ToList();
            if (!informationMessages.Any())
                return;

            foreach (var informationMessage in informationMessages)
            {
                MarkMessageAsOpenedAsync(informationMessage).FireAndForget();
            }
        }
    }
}
