using System;
using Foundation;
using RewriteMe.Mobile.Controls;
using RewriteMe.Mobile.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AlwaysScrollView), typeof(AlwaysScrollViewRenderer))]
namespace RewriteMe.Mobile.iOS.Renderers
{
    public class AlwaysScrollViewRenderer : ScrollViewRenderer
    {
        public AlwaysScrollViewRenderer()
        {
            var panGesture = new UIPanGestureRecognizer();

            panGesture.ShouldBegin = uiGestureRecognizer =>
            {
                var v = panGesture.VelocityInView(this);

                if (v.X != 0 || v.Y != 0)
                {
                    if (Math.Abs(v.X) > Math.Abs(v.Y))
                    {
                        return false;
                    }
                }

                return true;
            };

            AddGestureRecognizer(panGesture);
        }

        [Export("gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:")]
        public bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
        {
            return true;
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            AlwaysBounceVertical = true;
        }
    }
}