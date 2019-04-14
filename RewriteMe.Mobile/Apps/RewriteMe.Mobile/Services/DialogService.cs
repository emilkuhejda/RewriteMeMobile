using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Mobile.Utils;
using PromptResult = RewriteMe.Domain.Dialog.PromptResult;

namespace RewriteMe.Mobile.Services
{
    public class DialogService : IDialogService
    {
        public async Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null)
        {
            return await ThreadHelper.InvokeOnUiThread(() => UserDialogs.Instance.ConfirmAsync(message, title, okText, cancelText).ConfigureAwait(false));
        }

        public Task AlertAsync(string message, string title = null, string okText = null)
        {
            return ThreadHelper.InvokeOnUiThread(() => UserDialogs.Instance.AlertAsync(message, title, okText));
        }

        public async Task<PromptResult> PromptAsync(string message, string title = null, string okText = null, string cancelText = null, string placeholder = null, string text = null, Func<string, bool> inputValidation = null)
        {
            var result = await ThreadHelper.InvokeOnUiThread(() =>
            {
                var promptConfig = new PromptConfig
                {
                    Message = message,
                    Title = title,
                    OkText = okText,
                    CancelText = cancelText,
                    Placeholder = placeholder,
                    Text = text,
                    OnTextChanged = args =>
                    {
                        if (inputValidation != null)
                        {
                            args.IsValid = inputValidation(args.Value);
                        }
                    }
                };
                var acrPromtResult = UserDialogs.Instance.PromptAsync(promptConfig);
                return acrPromtResult;
            }).ConfigureAwait(false);

            return new PromptResult(result.Ok, result.Text);
        }
    }
}
