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