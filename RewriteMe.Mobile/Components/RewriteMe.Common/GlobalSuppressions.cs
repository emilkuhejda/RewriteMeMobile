// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Reliability", "CA2008:Do not create tasks without passing a TaskScheduler", Justification = "By design", Scope = "member", Target = "~M:RewriteMe.Common.Utils.AsyncHelper.RunSync(System.Func{System.Threading.Tasks.Task})")]
[assembly: SuppressMessage("Reliability", "CA2008:Do not create tasks without passing a TaskScheduler", Justification = "By design", Scope = "member", Target = "~M:RewriteMe.Common.Utils.AsyncHelper.RunSync``1(System.Func{System.Threading.Tasks.Task{``0}})~``0")]