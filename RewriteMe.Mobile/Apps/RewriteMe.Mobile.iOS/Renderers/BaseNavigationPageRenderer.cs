using System;
using RewriteMe.Mobile.iOS.Renderers;
using RewriteMe.Mobile.iOS.Utils;
using RewriteMe.Mobile.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BaseNavigationPage), typeof(BaseNavigationPageRenderer))]
namespace RewriteMe.Mobile.iOS.Renderers
{
    public class BaseNavigationPageRenderer : PageRenderer
    {
        private UIView _customStatusBarView;
        private nfloat _topSafeInset;
        private NSLayoutConstraint _customStatusBarHeightAnchor;

        /// <summary>
        /// Add custom status bar view because status bar view is no longer accessible in iOS 13.
        /// </summary>
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _customStatusBarView = new UIView();
            _topSafeInset = UIApplication.SharedApplication.Delegate.GetWindow().SafeAreaInsets.Top;
            _customStatusBarView.BackgroundColor = Colors.Primary;

            _customStatusBarView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(_customStatusBarView);

            _customStatusBarView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            _customStatusBarView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;

            _customStatusBarHeightAnchor = _customStatusBarView.HeightAnchor.ConstraintEqualTo(_topSafeInset);
            _customStatusBarHeightAnchor.Active = true;
        }

        public override void ViewSafeAreaInsetsDidChange()
        {
            base.ViewSafeAreaInsetsDidChange();

            _topSafeInset = UIApplication.SharedApplication.Delegate.GetWindow().SafeAreaInsets.Top;
            _customStatusBarHeightAnchor.Constant = _topSafeInset;
        }
    }
}