using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Customers.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Azure Mobile Services initilization
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            // SQLite initialization
            SQLitePCL.CurrentPlatform.Init();

            // Xamarin.Forms initialization
            Forms.Init();

            LoadApplication(new App());

            ConfigureTheming();

            return base.FinishedLaunching(app, options);
        }

        void ConfigureTheming()
        {
            UINavigationBar.Appearance.TintColor = UIColor.White;
            UINavigationBar.Appearance.BarTintColor = Color.FromHex("2C3E50").ToUIColor();
            UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes { ForegroundColor = UIColor.White };
            UIBarButtonItem.Appearance.SetTitleTextAttributes (new UITextAttributes { TextColor = UIColor.White }, UIControlState.Normal);
        }
    }
}

