﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IInformationMessageService
    {
        Task SynchronizationAsync(DateTime applicationUpdateDate);

        Task<IEnumerable<InformationMessage>> GetAllAsync();

        Task<bool> IsUnopenedMessageAsync();

        Task MarkAsOpenedAsync(InformationMessage informationMessage);
    }
}
