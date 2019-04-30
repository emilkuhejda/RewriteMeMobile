﻿using Prism.Ioc;
using RewriteMe.Business.Configuration;
using RewriteMe.Business.Factories;
using RewriteMe.Business.Services;
using RewriteMe.Business.Utils;
using RewriteMe.Common;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Factories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;

namespace RewriteMe.Business
{
    public class BusinessModule : IUnityModule
    {
        public void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IInternalValueService, InternalValueService>();
            containerRegistry.RegisterSingleton<IRewriteMeWebService, RewriteMeWebService>();
            containerRegistry.RegisterSingleton<IRegistrationUserWebService, RegistrationUserWebService>();
            containerRegistry.RegisterSingleton<IRewriteMeApiClientFactory, RewriteMeApiClientFactory>();
            containerRegistry.RegisterSingleton<IPublicClientApplicationFactory, PublicClientApplicationFactory>();
            containerRegistry.RegisterSingleton<IWebServiceErrorHandler, WebServiceErrorHandler>();
            containerRegistry.RegisterSingleton<IApplicationSettings, ApplicationSettings>();
            containerRegistry.RegisterSingleton<IUserSessionService, UserSessionService>();
            containerRegistry.RegisterSingleton<ILastUpdatesService, LastUpdatesService>();
            containerRegistry.RegisterSingleton<ISynchronizationService, SynchronizationService>();
            containerRegistry.RegisterSingleton<IFileItemService, FileItemService>();
            containerRegistry.RegisterSingleton<ITranscribeItemService, TranscribeItemService>();
            containerRegistry.RegisterSingleton<ICleanUpService, CleanUpService>();
        }
    }
}
