using System;

namespace RewriteMe.Domain.Configuration
{
    public static class InternalValues
    {
        public static InternalValue<string> ApplicationId { get; } = new InternalValue<string>("ApplicationId", null);

        public static InternalValue<DateTime> FileItemSynchronization { get; } = new InternalValue<DateTime>("FileItemSynchronization", default(DateTime));

        public static InternalValue<DateTime> DeletedFileItemSynchronization { get; } = new InternalValue<DateTime>("DeletedFileItemSynchronization", default(DateTime));

        public static InternalValue<DateTime> TranscribeItemSynchronization { get; } = new InternalValue<DateTime>("TranscribeItemSynchronization", default(DateTime));

        public static InternalValue<DateTime> UserSubscriptionSynchronization { get; } = new InternalValue<DateTime>("UserSubscriptionSynchronization", default(DateTime));

        public static InternalValue<string> LanguageSetting { get; } = new InternalValue<string>("LanguageSetting", null);

        public static InternalValue<bool> IsUserRegistrationSuccess { get; } = new InternalValue<bool>("IsUserRegistrationSuccess", false);

        public static InternalValue<string> DeletedFileItemsTotalTime { get; } = new InternalValue<string>("DeletedFileItemsTotalTime", null);
    }
}
