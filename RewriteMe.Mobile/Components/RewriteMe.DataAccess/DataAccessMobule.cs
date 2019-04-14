﻿using Prism.Ioc;
using RewriteMe.Common;
using RewriteMe.DataAccess.Providers;
using RewriteMe.DataAccess.Repositories;
using RewriteMe.Domain.Interfaces.Repositories;

namespace RewriteMe.DataAccess
{
    public class DataAccessMobule : IUnityModule
    {
        public void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppDbContext, AppDbContext>();
            containerRegistry.RegisterSingleton<IAppDbContextProvider, AppDbContextProvider>();
            containerRegistry.RegisterSingleton<IStorageInitializer, StorageInitializer>();
            containerRegistry.RegisterSingleton<IInternalValueRepository, InternalValueRepository>();
        }
    }
}