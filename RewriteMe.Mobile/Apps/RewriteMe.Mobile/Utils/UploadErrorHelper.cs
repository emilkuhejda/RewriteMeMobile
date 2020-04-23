using System.Net;
using RewriteMe.Domain.WebApi;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.Utils
{
    public static class UploadErrorHelper
    {
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
                case ErrorCode.EC200:
                    return Loc.Text(TranslationKeys.LanguageNotSupportedErrorMessage);
                case ErrorCode.EC201:
                    return Loc.Text(TranslationKeys.UploadedFileNotSupportedErrorMessage);
                case ErrorCode.EC300:
                    return Loc.Text(TranslationKeys.NotEnoughFreeMinutesInSubscriptionErrorMessage);
                case ErrorCode.EC400:
                    return Loc.Text(TranslationKeys.FileItemSourceDatabaseUpdateErrorMessage);
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
