namespace RewriteMe.Domain.Configuration
{
    public static class InternalValues
    {
        public static InternalValue<int?> FileItemSynchronization { get; } = new InternalValue<int?>("FileItemSynchronization");

        public static InternalValue<int?> AudioSourceSynchronization { get; } = new InternalValue<int?>("AudioSourceSynchronization");

        public static InternalValue<int?> TranscribeItemSynchronization { get; } = new InternalValue<int?>("TranscribeItemSynchronization");

        public static InternalValue<string> LanguageSetting { get; } = new InternalValue<string>("LanguageSetting", null);

        public static InternalValue<bool> IsUserRegistrationSuccess { get; } = new InternalValue<bool>("IsUserRegistrationSuccess", false);
    }
}
