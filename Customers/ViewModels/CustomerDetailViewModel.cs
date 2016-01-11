using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Linq;
using Plugin.Messaging;
using Plugin.ExternalMaps;
using Plugin.ExternalMaps.Abstractions;

namespace Customers
{
    public class CustomerDetailViewModel : BaseViewModel
    {
        bool _IsNewCustomer;

        readonly Geocoder _Geocoder;

        public CustomerDetailViewModel(Customer account = null)
        {
            _Geocoder = new Geocoder();

            if (account == null)
            {
                _IsNewCustomer = true;
                Account = new Customer();
            }
            else
            {
                _IsNewCustomer = false;
                Account = account;
            }

            SubscribeToSaveCustomerMessages();
        }

        public bool IsExistingCustomer { get { return !_IsNewCustomer; } }

        public string Title { get { return _IsNewCustomer ? "New Customer" : _Account.DisplayLastNameFirst; } }

        Customer _Account;

        public Customer Account
        {
            get { return _Account; }
            set
            {
                _Account = value;
                OnPropertyChanged("Account");
            }
        }

        bool _MapIsBusy = true;

        public bool MapIsBusy
        {
            get { return _MapIsBusy; }
            set
            {
                _MapIsBusy = value;
                OnPropertyChanged("MapIsBusy");
            }
        }

        Command _SaveCustomerCommand;

        /// <summary>
        /// Command to save customer
        /// </summary>
        public Command SaveCustomerCommand
        {
            get
            {
                return _SaveCustomerCommand ??
                (_SaveCustomerCommand = new Command(async () =>
                        await ExecuteSaveCustomerCommand()));
            }
        }

        async Task ExecuteSaveCustomerCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (String.IsNullOrWhiteSpace(Account.LastName) || String.IsNullOrWhiteSpace(Account.FirstName))
            {
                // display an alert, letting the user know that we require a first and last name in order to save a new customer
                await Page.DisplayAlert(
                    title: "Invalid name!", 
                    message: "A customer must have both a first and last name.",
                    cancel: "OK");
            }
            else
            {
                // send a message via MessagingCenter that we want the given customer to be saved
                MessagingCenter.Send(this.Account, "SaveCustomer");

                // perform a pop in order to navigate back to the customer list
                await Navigation.PopAsync();
            }

            IsBusy = false;
        }

        Command _EditCustomerCommand;

        /// <summary>
        /// Command to edit customer
        /// </summary>
        public Command EditCustomerCommand
        {
            get
            {
                return _EditCustomerCommand ??
                (_EditCustomerCommand = new Command(async () =>
                        await ExecuteEditCustomerCommand()));
            }
        }

        async Task ExecuteEditCustomerCommand()
        {
            var editPage = new CustomerEditPage();

            var viewmodel = this;

            viewmodel.Page = editPage;

            editPage.BindingContext = viewmodel;

            await Navigation.PushAsync(editPage);
        }


        Command _DeleteCustomerCommand;

        /// <summary>
        /// Command to delete customer
        /// </summary>
        public Command DeleteCustomerCommand
        {
            get
            {
                return _DeleteCustomerCommand ??
                (_DeleteCustomerCommand = new Command(async () =>
                        await ExecuteDeleteCustomerCommand()));
            }
        }

        async Task ExecuteDeleteCustomerCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            // display an alert and get the result of the user's decision
            var confirmDelete = 
                await Page.DisplayAlert(
                    title: String.Format("Delete {0}?", Account.DisplayName), 
                    message: null, 
                    accept: "Delete", 
                    cancel: "Cancel");

            if (confirmDelete)
            {
                // send a message via MessagingCenter that we want the given customer to be deleted
                MessagingCenter.Send(this.Account, "DeleteCustomer");

                // Performs two pops, not one. We want to navigate back to the list, not the detail screen.
                await Navigation.PopAsync(false); // passing false here to avoid two animations

                await Navigation.PopAsync();
            }

            IsBusy = false;
        }

        Command _DialNumberCommand;

        /// <summary>
        /// Command to dial customer phone number
        /// </summary>
        public Command DialNumberCommand
        {
            get
            {
                return _DialNumberCommand ??
                (_DialNumberCommand = new Command(async () =>
                        await ExecuteDialNumberCommand()));
            }
        }

        async Task ExecuteDialNumberCommand()
        {
            if (String.IsNullOrWhiteSpace(Account.Phone))
                return;      

            if (await Page.DisplayAlert(
                    title: $"Would you like to call {Account.DisplayName}?",
                    message: "",
                    accept: "Call",
                    cancel: "Cancel"))
            {
                var phoneCallTask = MessagingPlugin.PhoneDialer;
                if (phoneCallTask.CanMakePhoneCall)
                    phoneCallTask.MakePhoneCall(Account.Phone.SanitizePhoneNumber());
            }
        }

        Command _GetDirectionsCommand;

        public Command GetDirectionsCommand
        {
            get
            {
                return _GetDirectionsCommand ??
                (_GetDirectionsCommand = new Command(async() => 
                        await ExecuteGetDirectionsCommand()));
            }
        }

        async Task ExecuteGetDirectionsCommand()
        {
            if (await Page.DisplayAlert(
                    "Get Directions?", 
                    "Getting directions will take you out of this app. Continue to get directions?", 
                    "Yes", 
                    "Cancel"))
            {
                var position = await GetPosition();

                var pin = new Pin() { Position = position };

                CrossExternalMaps.Current.NavigateTo(pin.Label, pin.Position.Latitude, pin.Position.Longitude, NavigationType.Driving);
            }
        }

        public async Task<Position> GetPosition()
        {
            MapIsBusy = true;

            Position position;

            var positions = (await _Geocoder.GetPositionsForAddressAsync(Account.AddressString)).ToList();

            position = positions.FirstOrDefault();

            MapIsBusy = false;

            return position;
        }

        void SubscribeToSaveCustomerMessages()
        {
            // This subscribes to the "SaveCustomer" message
            MessagingCenter.Subscribe<Customer>(this, "SaveCustomer", async (customer) =>
                {
                    Account = customer;

                    // address has been updated, so we should update the map
                    if (Account.AddressString != customer.AddressString)
                    {
                        MessagingCenter.Send(this, "CustomerLocationUpdated");
                    }
                });
        }
    }
}

