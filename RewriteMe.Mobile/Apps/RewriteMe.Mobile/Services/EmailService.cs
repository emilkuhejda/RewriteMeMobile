﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RewriteMe.Domain.Interfaces.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendAsync(string recipient, string subject, string message, string attachmentFilePath)
        {
            var emailMessage = new EmailMessage
            {
                To = new List<string> { recipient },
                Subject = subject
            };

            if (Device.RuntimePlatform == Device.iOS)
            {
                var content = ReadContentFromFile(attachmentFilePath);
                emailMessage.Body = new StringBuilder(content).Append(message).ToString();
            }
            else
            {
                emailMessage.Body = message;
                emailMessage.Attachments.Add(new EmailAttachment(attachmentFilePath));
            }

            await Email.ComposeAsync(emailMessage).ConfigureAwait(false);
        }

        private string ReadContentFromFile(string attachmentFilePath)
        {
            var fileContent = string.Empty;
            if (File.Exists(attachmentFilePath))
            {
                var lines = File.ReadAllLines(attachmentFilePath).Reverse().Take(150).Reverse();
                fileContent = string.Join(Environment.NewLine, lines);
            }

            return fileContent;
        }

        public async Task SendAsync(string recipient, string subject, string message)
        {
            await Email.ComposeAsync(subject, message, recipient).ConfigureAwait(false);
        }
    }
}
