// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "By design", Scope = "member", Target = "~P:RewriteMe.Domain.Interfaces.Configuration.IApplicationSettings.RedirectUri")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "By design", Scope = "member", Target = "~P:RewriteMe.Domain.Interfaces.Configuration.IApplicationSettings.Scopes")]
[assembly: SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "By design", Scope = "member", Target = "~M:RewriteMe.Domain.Interfaces.Services.ISchedulerService.Stop")]
[assembly: SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "Name is OK", Scope = "member", Target = "~M:RewriteMe.Domain.Interfaces.Factories.IPublicClientApplicationFactory.CreatePublicClientApplication(System.String,System.String)~Microsoft.Identity.Client.IPublicClientApplication")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Array is OK", Scope = "member", Target = "~P:RewriteMe.Domain.Transcription.TranscriptAudioSource.Source")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Array is OK", Scope = "member", Target = "~P:RewriteMe.Domain.Transcription.RecordedAudioFile.Source")]