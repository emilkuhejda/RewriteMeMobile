using Android.Content;
using Android.Graphics;
using RewriteMe.Mobile.Droid.Renderers;
using RewriteMe.Mobile.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Editor), typeof(RewriteMeEditorRenderer))]
namespace RewriteMe.Mobile.Droid.Renderers
{
    public class RewriteMeEditorRenderer : EditorRenderer
    {
        public RewriteMeEditorRenderer(Context context)
            : base(context)
        { }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var customColor = ColorPalette.EditorBorder;
                Control.Background.SetColorFilter(customColor.ToAndroid(), PorterDuff.Mode.SrcAtop);
            }
        }
    }
}