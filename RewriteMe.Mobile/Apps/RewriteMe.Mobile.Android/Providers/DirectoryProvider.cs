using System;
using RewriteMe.Domain.Interfaces.Required;

namespace RewriteMe.Mobile.Droid.Providers
{
    public class DirectoryProvider : IDirectoryProvider
    {
        public string GetPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
    }
}