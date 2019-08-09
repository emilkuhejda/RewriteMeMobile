// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "By design", Scope = "type", Target = "~T:RewriteMe.Mobile.iOS.AppDelegate")]
[assembly: SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "By design", Scope = "type", Target = "~T:RewriteMe.Mobile.iOS.Renderers.NoBarsScrollViewerRenderer")]
[assembly: SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "By design", Scope = "type", Target = "~T:RewriteMe.Mobile.iOS.Renderers.AlwaysScrollViewRenderer")]
[assembly: SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "By design", Scope = "type", Target = "~T:RewriteMe.Mobile.iOS.Renderers.AlwaysScrollViewRenderer")]
[assembly: SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "By design", Scope = "type", Target = "~T:RewriteMe.Mobile.iOS.Renderers.NoBarsScrollViewerRenderer")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed with caller", Scope = "member", Target = "~M:RewriteMe.Mobile.iOS.Renderers.AlwaysScrollViewRenderer.#ctor")]
[assembly: SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "gestureRecognizer", Justification = "By design", Scope = "member", Target = "RewriteMe.Mobile.iOS.Renderers.AlwaysScrollViewRenderer.#ShouldRecognizeSimultaneously(UIKit.UIGestureRecognizer,UIKit.UIGestureRecognizer)")]
[assembly: SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "otherGestureRecognizer", Justification = "By design", Scope = "member", Target = "RewriteMe.Mobile.iOS.Renderers.AlwaysScrollViewRenderer.#ShouldRecognizeSimultaneously(UIKit.UIGestureRecognizer,UIKit.UIGestureRecognizer)")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "By design", Scope = "namespace", Target = "~N:RewriteMe.Mobile.iOS")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "By design", Scope = "type", Target = "~T:RewriteMe.Mobile.iOS.AppDelegate")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "By design", Scope = "namespace", Target = "~N:RewriteMe.Mobile.iOS.Configuration")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "By design", Scope = "namespace", Target = "~N:RewriteMe.Mobile.iOS.ExceptionHandling")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "By design", Scope = "namespace", Target = "~N:RewriteMe.Mobile.iOS.Localization")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:Access modifier should be declared", Justification = "By design", Scope = "member", Target = "~M:RewriteMe.Mobile.iOS.Application.Main(System.String[])")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "By design", Scope = "namespace", Target = "~N:RewriteMe.Mobile.iOS.Providers")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "By design", Scope = "namespace", Target = "~N:RewriteMe.Mobile.iOS.Renderers")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "By design", Scope = "namespace", Target = "~N:RewriteMe.Mobile.iOS.Services")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "By design", Scope = "member", Target = "~F:RewriteMe.Mobile.iOS.ExceptionHandling.NativeMethods.Signal.SIGSEGV")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1413:Use trailing comma in multi-line initializers", Justification = "By design", Scope = "member", Target = "~F:RewriteMe.Mobile.iOS.ExceptionHandling.NativeMethods.Signal.SIGSEGV")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "By design", Scope = "type", Target = "~T:RewriteMe.Mobile.iOS.ExceptionHandling.NativeMethods.Signal")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "By design", Scope = "member", Target = "~M:RewriteMe.Mobile.iOS.ExceptionHandling.NativeMethods.sigaction(RewriteMe.Mobile.iOS.ExceptionHandling.NativeMethods.Signal,System.IntPtr,System.IntPtr)~System.Int32")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "It is singleton", Scope = "member", Target = "~M:RewriteMe.Mobile.iOS.Logging.NLogLoggerConfiguration.Initialize(System.String)")]
[assembly: SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "By design", Scope = "type", Target = "~T:RewriteMe.Mobile.iOS.Renderers.RewriteMeEditorRenderer")]
[assembly: SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "By design", Scope = "type", Target = "~T:RewriteMe.Mobile.iOS.Renderers.RewriteMeEditorRenderer")]