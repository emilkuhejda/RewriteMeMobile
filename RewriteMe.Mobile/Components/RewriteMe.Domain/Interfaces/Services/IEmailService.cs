using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(string recipient, string subject, string message, string attachmentFilePath);

        Task SendAsync(string recipient, string subject, string message);
    }
}
