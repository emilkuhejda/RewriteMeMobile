namespace RewriteMe.Domain.Configuration
{
    public static class InternalValues
    {
        public static InternalValue<string> LanguageSetting { get; } = new InternalValue<string>("LanguageSetting", null);
    }
}
