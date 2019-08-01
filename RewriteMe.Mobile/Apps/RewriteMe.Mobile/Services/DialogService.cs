using System.Threading.Tasks;
using Acr.UserDialogs;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Mobile.Utils;

namespace RewriteMe.Mobile.Services
{
    public class DialogService : IDialogService
    {
        public Task AlertAsync(string message, string title = null, string okText = null)
        {
            return ThreadHelper.InvokeOnUiThread(() => UserDialogs.Instance.AlertAsync(message, title, okText));
        }

        public Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null)
        {
            return ThreadHelper.InvokeOnUiThread(() => UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Message = message,
                Title = title,
                OkText = okText,
                CancelText = cancelText
            }));
        }
    }
}
