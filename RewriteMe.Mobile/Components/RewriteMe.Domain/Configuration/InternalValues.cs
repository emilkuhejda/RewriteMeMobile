﻿namespace RewriteMe.Domain.Configuration
{
    public static class InternalValues
    {
        public static InternalValue<string> ApiUrl { get; } = new InternalValue<string>("ApiUrl", "https://voicipher.com/");

        public static InternalValue<bool> IsApplicationOutOfDate { get; } = new InternalValue<bool>("IsApplicationOutOfDate", false);

        public static InternalValue<string> ApplicationId { get; } = new InternalValue<string>("ApplicationId", null);

        public static InternalValue<long> FileItemSynchronizationTicks { get; } = new InternalValue<long>("FileItemSynchronizationTicks", 0);

        public static InternalValue<long> DeletedFileItemSynchronizationTicks { get; } = new InternalValue<long>("DeletedFileItemSynchronizationTicks", 0);

        public static InternalValue<long> TranscribeItemSynchronizationTicks { get; } = new InternalValue<long>("TranscribeItemSynchronizationTicks", 0);

        public static InternalValue<long> UserSubscriptionSynchronizationTicks { get; } = new InternalValue<long>("UserSubscriptionSynchronizationTicks", 0);

        public static InternalValue<long> InformationMessageSynchronizationTicks { get; } = new InternalValue<long>("InformationMessageSynchronizationTicks", 0);

        public static InternalValue<string> LanguageSetting { get; } = new InternalValue<string>("LanguageSetting", null);

        public static InternalValue<long> RemainingTimeTicks { get; } = new InternalValue<long>("RemainingTimeTicks", 0);

        public static InternalValue<bool> IsHighlightingEnabled { get; } = new InternalValue<bool>("IsHighlightingEnabled", false);

        public static InternalValue<bool> IsIntroSkipped { get; } = new InternalValue<bool>("IsIntroSkipped", false);
    }
}
