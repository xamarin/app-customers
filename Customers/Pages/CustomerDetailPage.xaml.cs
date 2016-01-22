using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;
using FormsToolkit;
using Xamarin;
using System;

namespace Customers
{
    public partial class CustomerDetailPage : ContentPage
    {
        protected CustomerDetailViewModel ViewModel
        {
            get { return BindingContext as CustomerDetailViewModel; }
        }

        public CustomerDetailPage()
        {
            InitializeComponent();
        }

        async protected override void OnAppearing()
        {
            base.OnAppearing();

            // Typically, is preferable to call into the viewmodel for OnAppearing() logic to be performed,
            // but we're not doing that in this case because we need to interact with the Xamarin.Forms.Map property on this Page.
            // In the future, the Map type and it's properties may get more binding support, so that the map setup can be omitted from code-behind.
            await SetupMap();
        }

        /// <summary>
        /// Sets up the map.
        /// </summary>
        /// <returns>A Task.</returns>
        async Task SetupMap()
        {
            if (ViewModel.HasAddress)
            {

                ViewModel.IsBusy = true;
                Map.IsVisible = false;

                // set to a default position
                Position position;

                try
                {
                    position = await ViewModel.GetPosition();
                }
                catch (Exception ex)
                {
                    MessagingService.Current.SendMessage(MessageKeys.DisplayGeocodingError);

                    ViewModel.IsBusy = false;

                    // TODO: Show insights
                    Insights.Report(ex, Insights.Severity.Error);

                    return;
                }

                // if lat and lon are both 0, then it's assumed that position acquisition failed
                if (position.Latitude == 0 && position.Longitude == 0)
                {
                    MessagingService.Current.SendMessage(MessageKeys.DisplayGeocodingError);

                    ViewModel.IsBusy = false;

                    return;
                }
                else
                {
                    var pin = new Pin()
                    { 
                        Type = PinType.Place, 
                        Position = position,
                        Label = ViewModel.Customer.DisplayName, 
                        Address = ViewModel.Customer.AddressString 
                    };

                    Map.Pins.Clear();

                    Map.Pins.Add(pin);

                    Map.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromMiles(10)));

                    Map.IsVisible = true;
                    ViewModel.IsBusy = false;
                }
            }
        }
    }
}

