using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Upload;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Business.Managers
{
    public class FileItemSourceUploader : SynchronizationManager, IFileItemSourceUploader
    {
        private readonly IFileItemService _fileItemService;
        private readonly IUploadedSourceService _uploadedSourceService;
        private readonly object _lockObject = new object();

        public FileItemSourceUploader(
            IFileItemService fileItemService,
            IUploadedSourceService uploadedSourceService)
        {
            _fileItemService = fileItemService;
            _uploadedSourceService = uploadedSourceService;

            _fileItemService.UploadProgress += HandleUploadProgress;
        }

        public UploadedFile CurrentUploadedFile { get; private set; }

        public async Task UploadAsync()
        {
            lock (_lockObject)
            {
                if (IsRunning)
                    return;

                IsRunning = true;
            }

            CurrentUploadedFile = null;

            await UploadInternalAsync().ConfigureAwait(false);

            CurrentUploadedFile = null;
            IsRunning = false;
        }

        private async Task UploadInternalAsync()
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource();

            var fileToUpload = await _uploadedSourceService.GetFirstAsync().ConfigureAwait(false);
            if (fileToUpload == null)
                return;

            CurrentUploadedFile = new UploadedFile(fileToUpload.FileItemId);

            try
            {
                await UploadSourceFileAsync(fileToUpload, CancellationTokenSource.Token).ConfigureAwait(false);

                if (fileToUpload.IsTranscript)
                {
                    await TranscribeAsync(fileToUpload, CancellationTokenSource.Token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                await _uploadedSourceService.DeleteAsync(fileToUpload.Id).ConfigureAwait(false);
            }

            await UploadInternalAsync().ConfigureAwait(false);
        }

        private async Task UploadSourceFileAsync(UploadedSource uploadedSource, CancellationToken cancellationToken)
        {
            var isUploadSuccess = false;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await UpdateUploadStatusAsync(uploadedSource.FileItemId, UploadStatus.InProgress, null).ConfigureAwait(false);
                await _fileItemService.DeleteChunksAsync(uploadedSource.FileItemId).ConfigureAwait(false);
                isUploadSuccess = await _fileItemService.UploadSourceFileAsync(uploadedSource.FileItemId, uploadedSource.Source, cancellationToken).ConfigureAwait(false);
                await UpdateUploadStatusAsync(uploadedSource.FileItemId, UploadStatus.Completed, null).ConfigureAwait(false);

                return;
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
            catch (FileChunkNotUploadedUploadException)
            {
                await UpdateUploadStatusAsync(uploadedSource.FileItemId, UploadStatus.Error, (int)HttpStatusCode.BadRequest).ConfigureAwait(false);
            }
            catch (NoSubscritionFreeTimeException)
            {
                await UpdateUploadStatusAsync(uploadedSource.FileItemId, UploadStatus.Error, (int)HttpStatusCode.MethodNotAllowed).ConfigureAwait(false);
            }
            catch (OfflineRequestException)
            {
                await UpdateUploadStatusAsync(uploadedSource.FileItemId, UploadStatus.Error, (int)HttpStatusCode.InternalServerError).ConfigureAwait(false);
            }
            finally
            {
                if (!isUploadSuccess)
                {
                    await _fileItemService.DeleteChunksAsync(uploadedSource.FileItemId).ConfigureAwait(false);
                }
            }

            CancellationTokenSource.Cancel();
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

        private void HandleUploadProgress(object sender, UploadProgressEventArgs e)
        {
            if (CurrentUploadedFile == null)
                return;

            if (CurrentUploadedFile.FileItemId != e.FileItemId)
                return;

            CurrentUploadedFile.Progress = e.PercentageDone;
        }
    }
}
