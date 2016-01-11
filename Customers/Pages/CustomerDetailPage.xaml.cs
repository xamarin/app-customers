using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;

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

            await SetupMap();
        }

        async Task SetupMap()
        {
            Map.IsVisible = false;

            // set to a default non-real position
            Position position = new Position(1000, 1000);

            try
            {
                position = await ViewModel.GetPosition();
            }
            catch
            {
                await DisplayGeocodingError();
            }

            // if lat and lon are both 1000, then it's assumed that position acquisition failed
            if (position.Latitude == 1000 && position.Longitude == 1000)
            {
                await DisplayGeocodingError();
            }
            else
            {
                var pin = new Pin()
                    { 
                        Type = PinType.Place, 
                        Position = position,
                        Label = ViewModel.Account.DisplayName, 
                        Address = ViewModel.Account.AddressString 
                    };

                Map.Pins.Clear();

                Map.Pins.Add(pin);

                Map.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromMiles(20)));

                Map.IsVisible = true;
            }
        }

        async Task DisplayGeocodingError()
        {
            await DisplayAlert(
                "Geocoding Error", 
                "Something went wrong while trying to translate the street address to GPS coordinates.", 
                "OK");
        }

        void SubscribeToCustomerLocationUpdatedMessages()
        {
            // update the map when receiving the CustomerLocationUpdated message
            MessagingCenter.Subscribe<Customer>(this, "CustomerLocationUpdated", async (customer) => await SetupMap());
        }
    }
}

