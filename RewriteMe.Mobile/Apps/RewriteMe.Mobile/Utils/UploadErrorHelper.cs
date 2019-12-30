using System.Net;
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
                    return Loc.Text(TranslationKeys.UploadedFileNotFoundErrorMessage);
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
    }
}
