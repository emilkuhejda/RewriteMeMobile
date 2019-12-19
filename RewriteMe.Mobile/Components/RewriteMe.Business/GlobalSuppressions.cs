// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "Name is OK", Scope = "member", Target = "~M:RewriteMe.Business.Factories.PublicClientApplicationFactory.CreatePublicClientApplication(System.String,System.String)~Microsoft.Identity.Client.IPublicClientApplication")]
[assembly: SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "Name is OK", Scope = "member", Target = "~P:RewriteMe.Business.Configuration.ApplicationSettings.RedirectUri")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Array is OK", Scope = "member", Target = "~P:RewriteMe.Business.Configuration.ApplicationSettings.Scopes")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1122:Use string.Empty for empty strings", Justification = "By design", Scope = "member", Target = "~P:RewriteMe.Business.Configuration.ApplicationSettings.Scopes")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Array is OK", Scope = "member", Target = "~P:RewriteMe.Business.Configuration.InitializationParameters.ImportedFileSource")]
[assembly: SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "By design", Scope = "member", Target = "~M:RewriteMe.Business.Extensions.TaskExtensions.FireAndForget(System.Threading.Tasks.Task)")]
[assembly: SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "It is singleton", Scope = "type", Target = "~T:RewriteMe.Business.Managers.TranscribeItemManager")]
[assembly: SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "It is singleton", Scope = "type", Target = "~T:RewriteMe.Business.Services.SynchronizerService")]
[assembly: SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "By design", Scope = "member", Target = "~P:RewriteMe.Business.Configuration.ApplicationSettings.WebApiUrl")]
