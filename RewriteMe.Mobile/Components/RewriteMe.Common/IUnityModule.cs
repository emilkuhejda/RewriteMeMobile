using Prism.Ioc;

namespace RewriteMe.Common
{
    public interface IUnityModule
    {
        void RegisterServices(IContainerRegistry containerRegistry);
    }
}
