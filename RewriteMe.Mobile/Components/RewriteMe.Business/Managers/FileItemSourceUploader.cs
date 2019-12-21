using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Managers
{
    public class FileItemSourceUploader : SynchronizationManager, IFileItemSourceUploader
    {
        private readonly IFileItemService _fileItemService;
        private readonly IUploadedSourceService _uploadedSourceService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly object _lockObject = new object();

        public FileItemSourceUploader(
            IFileItemService fileItemService,
            IUploadedSourceService uploadedSourceService,
            IRewriteMeWebService rewriteMeWebService)
        {
            _fileItemService = fileItemService;
            _uploadedSourceService = uploadedSourceService;
            _rewriteMeWebService = rewriteMeWebService;
        }

        public async Task UploadAsync()
        {
            lock (_lockObject)
            {
                if (IsRunning)
                    return;

                IsRunning = true;
            }

            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource();

            await UploadInternalAsync(CancellationTokenSource.Token).ConfigureAwait(false);

            IsRunning = false;
        }

        private async Task UploadInternalAsync(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            var fileToUpload = await _uploadedSourceService.GetFirstAsync().ConfigureAwait(false);
            if (fileToUpload == null)
                return;

            try
            {
                token.ThrowIfCancellationRequested();
                await UpdateUploadStatusAsync(fileToUpload.FileItemId, UploadStatus.InProgress, null).ConfigureAwait(false);
                var httpRequestResult = await _rewriteMeWebService.UploadSourceFileAsync(fileToUpload.FileItemId, fileToUpload.Source, token).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    await UpdateUploadStatusAsync(fileToUpload.FileItemId, UploadStatus.Completed, null).ConfigureAwait(false);

                    if (fileToUpload.IsTranscript)
                    {
                        token.ThrowIfCancellationRequested();
                        await TranscribeAsync(fileToUpload.FileItemId).ConfigureAwait(false);
                    }
                }
                else
                {
                    await UpdateUploadStatusAsync(fileToUpload.FileItemId, UploadStatus.Error, httpRequestResult.StatusCode).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                await UpdateUploadStatusAsync(fileToUpload.FileItemId, UploadStatus.Error, (int)HttpStatusCode.GatewayTimeout).ConfigureAwait(false);
            }
            catch (UnauthorizedCallException)
            {
                await UpdateUploadStatusAsync(fileToUpload.FileItemId, UploadStatus.Error, (int)HttpStatusCode.Unauthorized).ConfigureAwait(false);
                CancellationTokenSource.Cancel();

                OnUnauthorizedCallOccurred();
            }
            finally
            {
                await _uploadedSourceService.DeleteAsync(fileToUpload.Id).ConfigureAwait(false);
            }

            await UploadInternalAsync(token).ConfigureAwait(false);
        }

        private async Task TranscribeAsync(Guid fileItemId)
        {
            try
            {
                var fileItem = await _fileItemService.GetAsync(fileItemId).ConfigureAwait(false);
                await _fileItemService.TranscribeAsync(fileItem.Id, fileItem.Language).ConfigureAwait(false);
            }
            catch (ErrorRequestException)
            {
            }
            catch (NoSubscritionFreeTimeException)
            {

            }
            catch (OfflineRequestException)
            { }
        }

        private async Task UpdateUploadStatusAsync(Guid fileItemId, UploadStatus uploadStatus, int? errorCode)
        {
            await _fileItemService.UpdateUploadStatusAsync(fileItemId, uploadStatus).ConfigureAwait(false);
            await _fileItemService.SetUploadErrorCodeAsync(fileItemId, errorCode).ConfigureAwait(false);
        }
    }
}
