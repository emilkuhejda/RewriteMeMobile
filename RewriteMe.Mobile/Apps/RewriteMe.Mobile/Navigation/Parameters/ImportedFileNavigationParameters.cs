namespace RewriteMe.Mobile.Navigation.Parameters
{
    public class ImportedFileNavigationParameters
    {
        public ImportedFileNavigationParameters(string absolutePath, string path)
        {
            AbsolutePath = absolutePath;
            Path = path;
        }

        public string AbsolutePath { get; }

        public string Path { get; }
    }
}
