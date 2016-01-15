using System;
using System.IO;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Customers.UITests
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp
					.Android
                    .ApkFile ("../../../Droid/bin/Release/com.xamarin.customers-Signed.apk")
					.StartApp();
            }

            return ConfigureApp
				.iOS
                .AppBundle ("../../../iOS/bin/iPhoneSimulator/Debug/CustomersiOS.app")
				.StartApp();
        }
    }
}

