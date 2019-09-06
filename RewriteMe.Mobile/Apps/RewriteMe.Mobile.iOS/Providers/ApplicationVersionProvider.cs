using Foundation;
using Plugin.LatestVersion;
using RewriteMe.Domain.Interfaces.Required;

namespace RewriteMe.Mobile.iOS.Providers
{
    public class ApplicationVersionProvider : IApplicationVersionProvider
    {
        public string GetInstalledVersionNumber()
        {
            return $"{CrossLatestVersion.Current.InstalledVersionNumber}.{GetBuildNumber()}";
        }

        private string GetBuildNumber()
        {
            using (var bundleVersionString = new NSString("CFBundleVersion"))
            {
                return NSBundle.MainBundle.InfoDictionary.ValueForKey(bundleVersionString).ToString();
            }
        }
    }
}