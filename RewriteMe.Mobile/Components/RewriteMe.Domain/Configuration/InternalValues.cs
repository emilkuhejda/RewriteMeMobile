﻿using System;

namespace RewriteMe.Domain.Configuration
{
    public static class InternalValues
    {
        public static InternalValue<DateTime> FileItemSynchronization { get; } = new InternalValue<DateTime>("FileItemSynchronization", default(DateTime));

        public static InternalValue<DateTime> TranscribeItemSynchronization { get; } = new InternalValue<DateTime>("TranscribeItemSynchronization", default(DateTime));

        public static InternalValue<string> LanguageSetting { get; } = new InternalValue<string>("LanguageSetting", null);

        public static InternalValue<bool> IsUserRegistrationSuccess { get; } = new InternalValue<bool>("IsUserRegistrationSuccess", false);
    }
}
