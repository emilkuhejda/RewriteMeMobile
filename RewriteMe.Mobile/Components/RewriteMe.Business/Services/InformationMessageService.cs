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
        private const int DaysToDisplay = 7;

        private readonly IInternalValueService _internalValueService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly IInformationMessageRepository _informationMessageRepository;
        private readonly ILogger _logger;

        public event EventHandler MessageOpened;

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
        }

        public async Task<IEnumerable<InformationMessage>> GetAllForLastWeekAsync()
        {
            var minimumDateTime = DateTime.UtcNow.AddDays(DaysToDisplay);
            var informationMessages = await _informationMessageRepository.GetAllAsync(minimumDateTime).ConfigureAwait(false);

            await _informationMessageRepository.DeleteAsync(minimumDateTime).ConfigureAwait(false);
            return informationMessages;
        }

        public async Task<bool> HasUnopenedMessagesForLastWeekAsync()
        {
            var minimumDateTime = DateTime.UtcNow.AddDays(DaysToDisplay);
            return await _informationMessageRepository.HasUnopenedMessagesAsync(minimumDateTime).ConfigureAwait(false);
        }

        public async Task MarkAsOpenedAsync(InformationMessage informationMessage)
        {
            informationMessage.WasOpened = true;
            informationMessage.IsPendingSynchronization = false;

            await _informationMessageRepository.UpdateAsync(informationMessage).ConfigureAwait(false);

            if (informationMessage.IsUserSpecific)
            {
                MarkMessageAsOpenedOnServerAsync(informationMessage).FireAndForget();
            }

            OnMessageOpened();
        }

        private async Task MarkMessageAsOpenedOnServerAsync(InformationMessage informationMessage)
        {
            var httpRequestResult = await _rewriteMeWebService.MarkMessageAsOpenedAsync(informationMessage.Id).ConfigureAwait(false);
            if (httpRequestResult.State != HttpRequestState.Success)
            {
                informationMessage.IsPendingSynchronization = true;

                await _informationMessageRepository.UpdateAsync(informationMessage).ConfigureAwait(false);
            }
        }

        public async Task SendPendingAsync()
        {
            var pendingInformationMessages = await _informationMessageRepository.GetPendingAsync().ConfigureAwait(false);
            var informationMessages = pendingInformationMessages.ToList();
            if (!informationMessages.Any())
                return;

            _logger.Info($"Send pending information message open marks {informationMessages.Count} to server.");

            var ids = informationMessages.Select(x => (Guid?)x.Id);
            var httpRequestResult = await _rewriteMeWebService.MarkMessagesAsOpenedAsync(ids).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                foreach (var informationMessage in informationMessages)
                {
                    informationMessage.IsPendingSynchronization = false;
                    await _informationMessageRepository.UpdateAsync(informationMessage).ConfigureAwait(false);
                }
            }
        }

        private void OnMessageOpened()
        {
            MessageOpened?.Invoke(this, EventArgs.Empty);
        }
    }
}
