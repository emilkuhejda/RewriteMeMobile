using System.Diagnostics.CodeAnalysis;
using RewriteMe.Domain.WebApi;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.Utils
{
    public static class UploadErrorHelper
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By design")]
        public static string GetErrorMessage(ErrorCode? errorCode)
        {
            if (!errorCode.HasValue)
                return string.Empty;

            switch (errorCode)
            {
                case ErrorCode.EC100:
                case ErrorCode.EC101:
                case ErrorCode.EC102:
                    return Loc.Text(TranslationKeys.UploadedFileIsCorruptedErrorMessage);
                case ErrorCode.EC103:
                    return Loc.Text(TranslationKeys.FileIsAlreadyProcessingErrorMessage);
                case ErrorCode.EC104:
                    return Loc.Text(TranslationKeys.FileIsNotUploadedErrorMessage);
                case ErrorCode.EC105:
                    return Loc.Text(TranslationKeys.RecognizedAudioFileNotFoundErrorMessage);
                case ErrorCode.EC200:
                    return Loc.Text(TranslationKeys.LanguageNotSupportedErrorMessage);
                case ErrorCode.EC201:
                    return Loc.Text(TranslationKeys.UploadedFileNotSupportedErrorMessage);
                case ErrorCode.EC203:
                    return Loc.Text(TranslationKeys.PhoneCallModelNotSupportedErrorMessage);
                case ErrorCode.EC204:
                    return Loc.Text(TranslationKeys.StartTimeGreaterOrEqualThanEndTimeErrorMessage);
                case ErrorCode.EC205:
                    return Loc.Text(TranslationKeys.EndTimeGreaterThanTotalTimeErrorMessage);
                case ErrorCode.EC300:
                    return Loc.Text(TranslationKeys.NotEnoughFreeMinutesInSubscriptionErrorMessage);
                case ErrorCode.EC303:
                    return Loc.Text(TranslationKeys.OneTranscriptionAtTimeErrorMessage);
                case ErrorCode.EC304:
                    return Loc.Text(TranslationKeys.TooManyAttemptsToRestartTranscriptionErrorMessage);
                case ErrorCode.EC400:
                    return Loc.Text(TranslationKeys.FileItemSourceDatabaseUpdateErrorMessage);
                case ErrorCode.EC500:
                    return Loc.Text(TranslationKeys.SystemIsUnderMaintenanceErrorMessage);
                case ErrorCode.EC600:
                case ErrorCode.EC601:
                case ErrorCode.EC602:
                case ErrorCode.EC603:
                case ErrorCode.EC700:
                    return Loc.Text(TranslationKeys.SomethingWentWrongErrorMessage);
                case ErrorCode.EC800:
                    return Loc.Text(TranslationKeys.UploadedFileIsCorruptedErrorMessage);
                case ErrorCode.Unauthorized:
                    return Loc.Text(TranslationKeys.UnauthorizedErrorMessage);
                default:
                    return Loc.Text(TranslationKeys.UnreachableServerErrorMessage);
            }
        }
    }
}
