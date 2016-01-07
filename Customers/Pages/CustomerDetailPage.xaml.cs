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

            await ViewModel.LoadPins();

            var pin = new Pin()
            { 
                Type = PinType.Place, 
                Position = ViewModel.Position, 
                Label = ViewModel.Account.DisplayName, 
                Address = ViewModel.Account.AddressString 
            };

            Map.Pins.Add(pin);

            Map.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromMiles(20)));

            Map.IsVisible = true;
        }
    }
}

