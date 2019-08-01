using System.Threading.Tasks;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Mobile.Services
{
    public class EmailService : IEmailService
    {
        public bool CanSendEmail { get; }

        public Task SendAsync(string recipient, string subject, string message, string attachmentFilePath)
        {
        }

        public void Send(string recipient, string subject, string message)
        {
        }
    }
}
