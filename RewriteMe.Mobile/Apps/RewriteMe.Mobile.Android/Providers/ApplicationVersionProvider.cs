using Android.Content.PM;
using Plugin.CurrentActivity;
using Plugin.LatestVersion;
using RewriteMe.Domain.Interfaces.Required;

namespace RewriteMe.Mobile.Droid.Providers
{
    public class ApplicationVersionProvider : IApplicationVersionProvider
    {
        public string GetInstalledVersionNumber()
        {
            return $"{CrossLatestVersion.Current.InstalledVersionNumber}.{GetBuildNumber()}";
        }

        private string GetBuildNumber()
        {
            var context = CrossCurrentActivity.Current.AppContext;
            var packageInfo = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData);

            return packageInfo.VersionCode.ToString();
        }
    }
}