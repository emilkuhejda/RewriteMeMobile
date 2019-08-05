using System;

namespace RewriteMe.Domain.Configuration
{
    public static class InternalValues
    {
        public static InternalValue<string> ApplicationId { get; } = new InternalValue<string>("ApplicationId", null);

        public static InternalValue<long> FileItemSynchronizationTicks { get; } = new InternalValue<long>("FileItemSynchronizationTicks", 0);

        public static InternalValue<long> DeletedFileItemSynchronizationTicks { get; } = new InternalValue<long>("DeletedFileItemSynchronizationTicks", 0);

        public static InternalValue<long> TranscribeItemSynchronizationTicks { get; } = new InternalValue<long>("TranscribeItemSynchronizationTicks", 0);

        public static InternalValue<long> UserSubscriptionSynchronizationTicks { get; } = new InternalValue<long>("UserSubscriptionSynchronizationTicks", 0);

        public static InternalValue<string> LanguageSetting { get; } = new InternalValue<string>("LanguageSetting", null);

        public static InternalValue<bool> IsUserRegistrationSuccess { get; } = new InternalValue<bool>("IsUserRegistrationSuccess", false);

        public static InternalValue<long> DeletedFileItemsTotalTime { get; } = new InternalValue<long>("DeletedFileItemsTotalTime", 0);

        public static InternalValue<long> RecognizedTimeTicks { get; } = new InternalValue<long>("RecognizedTimeTicks", 0);
    }
}
