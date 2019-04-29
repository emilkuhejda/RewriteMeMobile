namespace RewriteMe.Domain.Configuration
{
    public static class InternalValues
    {
        public static InternalValue<string> LanguageSetting { get; } = new InternalValue<string>("LanguageSetting", null);

        public static InternalValue<bool> IsUserRegistrationSuccess { get; } = new InternalValue<bool>("IsUserRegistrationSuccess", false);
    }
}
