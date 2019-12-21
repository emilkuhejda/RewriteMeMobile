using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.Upload;

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

        private async Task UploadInternalAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var fileToUpload = await _uploadedSourceService.GetFirstAsync().ConfigureAwait(false);
            if (fileToUpload == null)
                return;

            try
            {
                await UploadAsync(fileToUpload, cancellationToken).ConfigureAwait(false);

                if (fileToUpload.IsTranscript)
                {
                    await TranscribeAsync(fileToUpload, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                await _uploadedSourceService.DeleteAsync(fileToUpload.Id).ConfigureAwait(false);
            }

            await UploadInternalAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task UploadAsync(UploadedSource uploadedSource, CancellationToken cancellationToken)
        {
            var mediaFile = new MediaFile
            {
                Name = uploadedSource.Name,
                Language = uploadedSource.Language,
                FileName = uploadedSource.FileName,
                Source = uploadedSource.Source
            };

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await UpdateUploadStatusAsync(uploadedSource.FileItemId, UploadStatus.InProgress, null).ConfigureAwait(false);
                await _fileItemService.UploadAsync(mediaFile, cancellationToken).ConfigureAwait(false);
                await UpdateUploadStatusAsync(uploadedSource.FileItemId, UploadStatus.Completed, null).ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException)
            {
                await UpdateUploadStatusAsync(uploadedSource.FileItemId, UploadStatus.Error, (int)HttpStatusCode.Unauthorized).ConfigureAwait(false);
                CancellationTokenSource.Cancel();

                OnUnauthorizedCallOccurred();
            }
            catch (ErrorRequestException ex)
            {
                await UpdateUploadStatusAsync(uploadedSource.FileItemId, UploadStatus.Error, ex.StatusCode).ConfigureAwait(false);
            }
            catch (NoSubscritionFreeTimeException)
            {
                await UpdateUploadStatusAsync(uploadedSource.FileItemId, UploadStatus.Error, (int)HttpStatusCode.Conflict).ConfigureAwait(false);
            }
            catch (OfflineRequestException)
            {
                await UpdateUploadStatusAsync(uploadedSource.FileItemId, UploadStatus.Error, (int)HttpStatusCode.InternalServerError).ConfigureAwait(false);
            }
        }

        private async Task TranscribeAsync(UploadedSource uploadedSource, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await _fileItemService.SetTranscribeErrorCodeAsync(uploadedSource.FileItemId, null).ConfigureAwait(false);
                await _fileItemService.TranscribeAsync(uploadedSource.FileItemId, uploadedSource.Language).ConfigureAwait(false);
            }
            catch (ErrorRequestException ex)
            {
                await _fileItemService.SetTranscribeErrorCodeAsync(uploadedSource.FileItemId, ex.StatusCode).ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException)
            {
                await _fileItemService.SetTranscribeErrorCodeAsync(uploadedSource.FileItemId, (int)HttpStatusCode.Unauthorized).ConfigureAwait(false);
                CancellationTokenSource.Cancel();

                OnUnauthorizedCallOccurred();
            }
            catch (NoSubscritionFreeTimeException)
            {
                await _fileItemService.SetTranscribeErrorCodeAsync(uploadedSource.FileItemId, (int)HttpStatusCode.Conflict).ConfigureAwait(false);
            }
            catch (OfflineRequestException)
            {
                await _fileItemService.SetTranscribeErrorCodeAsync(uploadedSource.FileItemId, (int)HttpStatusCode.InternalServerError).ConfigureAwait(false);
            }
        }

        private async Task UpdateUploadStatusAsync(Guid fileItemId, UploadStatus uploadStatus, int? errorCode)
        {
            await _fileItemService.UpdateUploadStatusAsync(fileItemId, uploadStatus).ConfigureAwait(false);
            await _fileItemService.SetUploadErrorCodeAsync(fileItemId, errorCode).ConfigureAwait(false);
        }
    }
}
