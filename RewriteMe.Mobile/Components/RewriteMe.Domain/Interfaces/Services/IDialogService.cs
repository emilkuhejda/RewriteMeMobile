using System.Threading.Tasks;
using RewriteMe.Domain.Dialog;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IDialogService
    {
        Task AlertAsync(string message, string title = null, string okText = null);

        Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null);

        Task<PromptDialogResult> PromptAsync(string message, string title = null, string okText = null, string cancelText = null);
    }
}
