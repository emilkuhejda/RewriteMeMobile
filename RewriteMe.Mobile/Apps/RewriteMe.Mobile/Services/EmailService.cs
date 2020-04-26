using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Messaging;
using RewriteMe.Domain.Interfaces.Services;
using Xamarin.Essentials;
using EmailAttachment = Xamarin.Essentials.EmailAttachment;

namespace RewriteMe.Mobile.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailTask _emailTask;

        public EmailService(IEmailTask emailTask)
        {
            _emailTask = emailTask;
        }

        public bool CanSendEmail => _emailTask.CanSendEmail;

        public async Task SendAsync(string recipient, string subject, string message, string attachmentFilePath)
        {
            var emailMessage = new EmailMessage
            {
                To = new List<string> { recipient },
                Subject = "Hello",
                Body = "World",
            };

            emailMessage.Attachments.Add(new EmailAttachment(attachmentFilePath));

            await Email.ComposeAsync(emailMessage).ConfigureAwait(false);
        }

        public async Task SendAsync(string recipient, string subject, string message)
        {
            await Email.ComposeAsync(subject, message, recipient).ConfigureAwait(false);
        }
    }
}
