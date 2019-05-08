// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "By design", Scope = "member", Target = "~P:RewriteMe.Mobile.ViewModels.DetailPageViewModel.TranscribeItems")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Array is OK", Scope = "member", Target = "~P:RewriteMe.Mobile.Transcription.SupportedLanguages.All")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "It is disposed with caller", Scope = "member", Target = "~M:RewriteMe.Mobile.Utils.ThreadHelper.InvokeOnUiThread(System.Action)")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "It is disposed with caller", Scope = "member", Target = "~M:RewriteMe.Mobile.Utils.ThreadHelper.InvokeOnUiThread``1(System.Func{``0})~``0")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Array is OK", Scope = "member", Target = "~P:RewriteMe.Mobile.ViewModels.OverviewPageViewModel.FileItems")]