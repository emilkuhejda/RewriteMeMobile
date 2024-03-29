﻿using Prism.Ioc;
using RewriteMe.Domain.Interfaces.ExceptionHandling;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Configuration;
using RewriteMe.Mobile.iOS.ExceptionHandling;
using RewriteMe.Mobile.iOS.Localization;
using RewriteMe.Mobile.iOS.Logging;
using RewriteMe.Mobile.iOS.Providers;
using RewriteMe.Mobile.iOS.Services;

namespace RewriteMe.Mobile.iOS.Configuration
{
    public class OsxBootstrapper : Bootstrapper
    {
        protected override void RegisterPlatformServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILocalizer, Localizer>();
            containerRegistry.RegisterSingleton<IDirectoryProvider, DirectoryProvider>();
            containerRegistry.RegisterSingleton<IIdentityUiParentProvider, IdentityUiParentProvider>();
            containerRegistry.RegisterSingleton<IApplicationVersionProvider, ApplicationVersionProvider>();
            containerRegistry.RegisterSingleton<ILoggerConfiguration, NLogLoggerConfiguration>();
            containerRegistry.RegisterSingleton<ILogFileReader, NLogFileReader>();
            containerRegistry.RegisterSingleton<ILoggerFactory, NLogLoggerFactory>();
            containerRegistry.RegisterSingleton<IExceptionHandler, ExceptionHandler>();
            containerRegistry.RegisterSingleton<IMediaService, MediaService>();
            containerRegistry.RegisterSingleton<IScreenService, ScreenService>();
        }
    }
}