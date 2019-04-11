using System;
using System.IO;
using RewriteMe.Domain.Interfaces.Required;

namespace RewriteMe.Mobile.iOS.Providers
{
    public class DirectoryProvider : IDirectoryProvider
    {
        public string GetPath()
        {
            const string libraryName = "Library";

            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(documentsPath, "..", libraryName);
        }
    }
}