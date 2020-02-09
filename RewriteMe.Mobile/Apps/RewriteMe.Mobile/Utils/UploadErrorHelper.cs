using System.Net;
using RewriteMe.Domain.WebApi;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.Utils
{
    public static class UploadErrorHelper
    {
        public static string GetErrorMessage(int? statusCode)
        {
            switch (statusCode)
            {
                case (int)HttpStatusCode.BadRequest:
                    return Loc.Text(TranslationKeys.UploadedFileIsCorruptedErrorMessage);
                case (int)HttpStatusCode.Unauthorized:
                    return Loc.Text(TranslationKeys.UnauthorizedErrorMessage);
                case (int)HttpStatusCode.MethodNotAllowed:
                    return Loc.Text(TranslationKeys.NotEnoughFreeMinutesInSubscriptionErrorMessage);
                case (int)HttpStatusCode.NotAcceptable:
                    return Loc.Text(TranslationKeys.LanguageNotSupportedErrorMessage);
                case (int)HttpStatusCode.Conflict:
                    return Loc.Text(TranslationKeys.FileItemSourceDatabaseUpdateErrorMessage);
                case (int)HttpStatusCode.UnsupportedMediaType:
                    return Loc.Text(TranslationKeys.UploadedFileNotSupportedErrorMessage);
                default:
                    return Loc.Text(TranslationKeys.UnreachableServerErrorMessage);
            }
        }

        public static string GetErrorMessage(ErrorCode errorCode)
        {
            switch (errorCode)
            {
                case ErrorCode.EC1:
                case ErrorCode.EC2:
                    return Loc.Text(TranslationKeys.UploadedFileIsCorruptedErrorMessage);
                case ErrorCode.EC3:
                    return Loc.Text(TranslationKeys.LanguageNotSupportedErrorMessage);
                case ErrorCode.EC4:
                    return Loc.Text(TranslationKeys.UploadedFileNotSupportedErrorMessage);
                case ErrorCode.EC5:
                    return Loc.Text(TranslationKeys.FileItemSourceDatabaseUpdateErrorMessage);
                case ErrorCode.EC6:
                    return Loc.Text(TranslationKeys.NotEnoughFreeMinutesInSubscriptionErrorMessage);
                default:
                    return Loc.Text(TranslationKeys.UnreachableServerErrorMessage);
            }
        }
    }
}
