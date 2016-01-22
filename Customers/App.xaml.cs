using Xamarin.Forms;
using FormsToolkit;

namespace Customers
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            SubscribeToDisplayAlertMessages();

            if (Device.OS == TargetPlatform.iOS)
                navPage.BarTextColor = Color.White;
        }

        /// <summary>
        /// Subscribes to messages for displaying alerts.
        /// </summary>
        static void SubscribeToDisplayAlertMessages()
        {
            MessagingService.Current.Subscribe<MessagingServiceAlert>(MessageKeys.DisplayAlert, async (service, info) =>
                {
                    var task = Application.Current?.MainPage?.DisplayAlert(info.Title, info.Message, info.Cancel);
                    if (task != null)
                    {
                        await task;
                        info?.OnCompleted?.Invoke();
                    }
                });

            MessagingService.Current.Subscribe<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, async (service, info) =>
                {
                    var task = Application.Current?.MainPage?.DisplayAlert(info.Title, info.Question, info.Positive, info.Negative);
                    if (task != null)
                    {
                        var result = await task;
                        info?.OnCompleted?.Invoke(result);
                    }
                });

            MessagingService.Current.Subscribe(MessageKeys.DisplayGeocodingError, async (service) =>
                {
                    var task = Application.Current?.MainPage?.DisplayAlert("Geocoding Error", "An eror occurred while converting the street address to GPS coordinates.", "OK");
                    if (task != null)
                        await task;
                });
        }
    }
}

