// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Array is OK", Scope = "member", Target = "~P:RewriteMe.Mobile.Transcription.SupportedLanguages.All")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "It is disposed with caller", Scope = "member", Target = "~M:RewriteMe.Mobile.Utils.ThreadHelper.InvokeOnUiThread(System.Action)")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "It is disposed with caller", Scope = "member", Target = "~M:RewriteMe.Mobile.Utils.ThreadHelper.InvokeOnUiThread``1(System.Func{``0})~``0")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Array is OK", Scope = "member", Target = "~P:RewriteMe.Mobile.ViewModels.OverviewPageViewModel.FileItems")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1119:Statement must not use unnecessary parenthesis", Justification = "By design", Scope = "member", Target = "~M:RewriteMe.Mobile.Controls.StackedList.AddGesture(Xamarin.Forms.View,Xamarin.Forms.TapGestureRecognizer)")]
[assembly: SuppressMessage("Reliability", "CA2008:Do not create tasks without passing a TaskScheduler", Justification = "By design", Scope = "member", Target = "~M:RewriteMe.Mobile.Commands.AsyncCommand.ExecuteAsync~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Array is OK", Scope = "member", Target = "~P:RewriteMe.Mobile.Transcription.SubscriptionProducts.All")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "By design", Scope = "member", Target = "~M:RewriteMe.Mobile.ViewModels.UserSubscriptionsPageViewModel.MakePurchaseAsync(System.String)~System.Threading.Tasks.Task{System.Boolean}")]
[assembly: SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "By design", Scope = "member", Target = "~M:RewriteMe.Mobile.ViewModels.ViewModelBase.Dispose(System.Boolean)")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "By design", Scope = "member", Target = "~P:RewriteMe.Mobile.ViewModels.DetailBaseViewModel`1.DetailItems")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "By design", Scope = "member", Target = "~M:RewriteMe.Mobile.ViewModels.DeveloperPageViewModel.ExecuteSendLogMailCommandAsync~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "By design", Scope = "member", Target = "~P:RewriteMe.Mobile.Transcription.PickedFile.Source")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "By design", Scope = "member", Target = "~P:RewriteMe.Mobile.Navigation.Parameters.ImportedFileNavigationParameters.Source")]