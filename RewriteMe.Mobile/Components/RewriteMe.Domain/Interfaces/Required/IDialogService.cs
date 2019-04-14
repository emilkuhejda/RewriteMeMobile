using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Dialog;

namespace RewriteMe.Domain.Interfaces.Required
{
    public interface IDialogService
    {
        Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null);

        Task AlertAsync(string message, string title = null, string okText = null);

        Task<PromptResult> PromptAsync(string message, string title = null, string okText = null, string cancelText = null, string placeholder = null, string text = null, Func<string, bool> inputValidation = null);
    }
}
