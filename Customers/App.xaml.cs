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

            customerListPage.BindingContext = new CustomerListViewModel();
        }

        static void SubscribeToDisplayAlertMessages()
        {
            MessagingService.Current.Subscribe<MessagingServiceAlert>(MessageKeys.DisplayAlert, async (service, info) =>
                {
                    var task = Application.Current?.MainPage?.DisplayAlert(info.Title, info.Message, info.Cancel);
                    if (task != null)
                        await task;
                });

            MessagingService.Current.Subscribe<MessagingServiceQuestionWithAction>(MessageKeys.DisplayQuestion, async (service, info) =>
                {
                    var task = Application.Current?.MainPage?.DisplayAlert(info.Title, info.Question, info.Positive, info.Negative);
                    if (task != null && await task)
                        info?.SuccessAction?.Invoke();
                });

            MessagingService.Current.Subscribe(MessageKeys.PopAsync, async (service) =>
                {
                    var task = Application.Current?.MainPage?.Navigation?.PopAsync();
                    if (task != null)
                        await task;
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

