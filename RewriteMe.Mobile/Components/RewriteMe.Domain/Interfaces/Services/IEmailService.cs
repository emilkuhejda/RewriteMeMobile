﻿using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IEmailService
    {
        bool CanSendEmail { get; }

        Task SendAsync(string recipient, string subject, string message, string attachmentFilePath);

        Task SendAsync(string recipient, string subject, string message);
    }
}
