﻿// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed with caller", Scope = "member", Target = "~M:RewriteMe.Mobile.Droid.SplashActivity.OnCreate(Android.OS.Bundle)")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "By design", Scope = "member", Target = "~M:RewriteMe.Mobile.Droid.Logging.NLogLoggerConfiguration.Initialize(System.String)")]
[assembly: SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "By design", Scope = "member", Target = "~P:RewriteMe.Mobile.Droid.Utils.ExtraConstants.FileUri")]