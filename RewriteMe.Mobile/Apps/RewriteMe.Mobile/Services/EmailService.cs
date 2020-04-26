using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Messaging;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Mobile.Utils;
using Xamarin.Essentials;
using Xamarin.Forms;
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
            if (Device.RuntimePlatform == Device.iOS)
            {
                if (_emailTask.CanSendEmailAttachments)
                {
                    SendWithAttachment(recipient, subject, message, attachmentFilePath);
                }
                else
                {
                    SendWithoutAttachment(recipient, subject, message, attachmentFilePath);
                }
            }
            else
            {
                var email = new EmailMessage
                {
                    To = new List<string> { recipient },
                    Subject = subject,
                    Body = message
                };

                if (File.Exists(attachmentFilePath))
                {
                    email.Attachments.Add(new EmailAttachment(attachmentFilePath));
                }

                await ThreadHelper.InvokeOnUiThread(async () =>
                {
                    await Email.ComposeAsync(email).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
        }

        private void SendWithAttachment(string recipient, string subject, string message, string attachmentFilePath)
        {
            var emailMessage = new EmailMessageBuilder()
                .To(recipient)
                .Subject(subject)
                .Body(message)
                .WithAttachment(attachmentFilePath, "text/plain")
                .Build();

            ThreadHelper.InvokeOnUiThread(() =>
            {
                _emailTask.SendEmail(emailMessage);
            });
        }

        private void SendWithoutAttachment(string recipient, string subject, string message, string attachmentFilePath)
        {
            var fileContent = string.Empty;
            if (File.Exists(attachmentFilePath))
            {
                var lines = File.ReadAllLines(attachmentFilePath).Reverse().Take(150).Reverse();
                fileContent = string.Join(Environment.NewLine, lines);
            }

            var body = new StringBuilder(fileContent).Append(message).ToString();

            var emailMessage = new EmailMessageBuilder()
                .To(recipient)
                .Subject(subject)
                .Body(body)
                .Build();

            ThreadHelper.InvokeOnUiThread(() =>
            {
                _emailTask.SendEmail(emailMessage);
            });
        }

        public async Task SendAsync(string recipient, string subject, string message)
        {
            await Email.ComposeAsync(subject, message, recipient).ConfigureAwait(false);
        }
    }
}
