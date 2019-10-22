using RewriteMe.Mobile.Controls;
using RewriteMe.Mobile.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(RewriteMeWebView), typeof(RewriteMeWebViewRenderer))]
namespace RewriteMe.Mobile.iOS.Renderers
{
    public class RewriteMeWebViewRenderer : WkWebViewRenderer
    {
    }
}