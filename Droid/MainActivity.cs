using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Droid;

namespace Customers.Droid
{
    [Activity(Label = "Customers.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity // inhertiting from FormsAppCompatActivity is imperative to taking advantage of Android AppCompat libraries
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Forms.Init(this, bundle);

            // this line is essential to wiring up the toolbar styles defined in ~/Resources/layout/toolbar.axml
            FormsAppCompatActivity.ToolbarResource = Resource.Layout.toolbar;

            ImageCircleRenderer.Init();

            LoadApplication(new App());
        }
    }
}

