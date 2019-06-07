namespace RewriteMe.Business.Configuration
{
    public class InitializationParameters
    {
        private static InitializationParameters _current;

        public static InitializationParameters Current
        {
            get
            {
                if (_current == null)
                    _current = new InitializationParameters();

                return _current;
            }
        }

        public string ImportedFilePath { get; set; }
    }
}
