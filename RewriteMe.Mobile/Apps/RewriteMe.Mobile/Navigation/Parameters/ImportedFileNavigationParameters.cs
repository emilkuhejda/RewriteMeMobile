namespace RewriteMe.Mobile.Navigation.Parameters
{
    public class ImportedFileNavigationParameters
    {
        public ImportedFileNavigationParameters(string fileName, byte[] source)
        {
            FileName = fileName;
            Source = source;
        }

        public string FileName { get; }

        public byte[] Source { get; }
    }
}
