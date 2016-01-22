using System;
using System.Linq;
using System.Threading.Tasks;
using FormsToolkit;
using Plugin.ExternalMaps;
using Plugin.ExternalMaps.Abstractions;
using Plugin.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Customers
{
    public class CustomerDetailViewModel : BaseNavigationViewModel
    {
        bool _IsNewCustomer;

        // this is just a utility service that we're using in this demo app to mitigate some limitations of the iOS simulator
        ICapabilityService _CapabilityService;

        readonly Geocoder _Geocoder;

        public CustomerDetailViewModel(Customer customer = null)
        {
            _CapabilityService = DependencyService.Get<ICapabilityService>();

            _Geocoder = new Geocoder();

            if (customer == null)
            {
                _IsNewCustomer = true;
                Customer = new Customer();
            }
            else
            {
                _IsNewCustomer = false;
                Customer = customer;
            }

            Title = _IsNewCustomer ? "New Customer" : _Customer.DisplayLastNameFirst;

            _AddressString = Customer.AddressString;

            SubscribeToSaveCustomerMessages();

            SubscribeToCustomerLocationUpdatedMessages();
        }

        public bool IsExistingCustomer { get { return !_IsNewCustomer; } }

        public bool HasEmailAddress { get { return Customer != null && !String.IsNullOrWhiteSpace(Customer.Email); } }

        public bool HasPhoneNumber { get { return Customer != null && !String.IsNullOrWhiteSpace(Customer.Phone); } }

        public bool HasAddress { get { return Customer != null && !String.IsNullOrWhiteSpace(Customer.AddressString); } }

        private string _AddressString;

        Customer _Customer;

        public Customer Customer
        {
            get { return _Customer; }
            set
            {
                _Customer = value;
                OnPropertyChanged("Customer");
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
                    (_SaveCustomerCommand = new Command(() => ExecuteSaveCustomerCommand()));
            }
        }

        async Task ExecuteSaveCustomerCommand()
        {
            if (String.IsNullOrWhiteSpace(Customer.LastName) || String.IsNullOrWhiteSpace(Customer.FirstName))
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Invalid name!", 
                        Message = "A customer must have both a first and last name.",
                        Cancel = "OK"
                    });
            }
            else if (!RequiredAddressFieldCombinationIsFilled)
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Invalid address!", 
                        Message = "You must enter either a street, city, and state combination, or a postal code.",
                        Cancel = "OK"
                    });
            }
            else
            {
                MessagingService.Current.SendMessage<Customer>(MessageKeys.SaveCustomer, this.Customer);

                await PopAsync();
            }
        }

        bool RequiredAddressFieldCombinationIsFilled
        {
            get
            {
                if (Customer.AddressString.IsNullOrWhiteSpace())
                {
                    return true;
                }
                else if (!Customer.Street.IsNullOrWhiteSpace() && (!Customer.City.IsNullOrWhiteSpace() && !Customer.State.IsNullOrWhiteSpace()))
                {
                    return true;
                }
                else if (!Customer.PostalCode.IsNullOrWhiteSpace() && (Customer.Street.IsNullOrWhiteSpace() || Customer.City.IsNullOrWhiteSpace() || Customer.State.IsNullOrWhiteSpace()))
                {
                    return true;
                }

                return false;
            }
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
                    (_EditCustomerCommand = new Command(async () => await ExecuteEditCustomerCommand()));
            }
        }

        async Task ExecuteEditCustomerCommand()
        {
            await PushAsync(new CustomerEditPage() { BindingContext = this });
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
                    (_DeleteCustomerCommand = new Command(ExecuteDeleteCustomerCommand));
            }
        }

        void ExecuteDeleteCustomerCommand()
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
                {
                    Title = String.Format("Delete {0}?", Customer.DisplayName),
                    Question = null,
                    Positive = "Delete",
                    Negative = "Cancel",
                    OnCompleted = new Action<bool>(async result =>
                        {
                            if (result)
                            {
                                await PopAsync(false);

                                await PopAsync();

                                // send a message that we want the given customer to be deleted
                                MessagingService.Current.SendMessage<Customer>(MessageKeys.DeleteCustomer, this.Customer);
                            }
                        })
                });
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
                (_DialNumberCommand = new Command(ExecuteDialNumberCommand));
            }
        }

        void ExecuteDialNumberCommand()
        {
            if (String.IsNullOrWhiteSpace(Customer.Phone))
                return;

            if (_CapabilityService.CanMakeCalls)
            {
                var phoneCallTask = MessagingPlugin.PhoneDialer;
                if (phoneCallTask.CanMakePhoneCall)
                    phoneCallTask.MakePhoneCall(Customer.Phone.SanitizePhoneNumber());
            }
            else
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Simulator Not Supported", 
                        Message = "Phone calls are not supported in the iOS simulator.",
                        Cancel = "OK"
                    });
            }
        }

        Command _MessageNumberCommand;

        /// <summary>
        /// Command to message customer phone number
        /// </summary>
        public Command MessageNumberCommand
        {
            get
            {
                return _MessageNumberCommand ??
                (_MessageNumberCommand = new Command(ExecuteMessageNumberCommand));
            }
        }

        void ExecuteMessageNumberCommand()
        {
            if (String.IsNullOrWhiteSpace(Customer.Phone))
                return;

            if (_CapabilityService.CanSendMessages)
            {
                var messageTask = MessagingPlugin.SmsMessenger;
                if (messageTask.CanSendSms)
                    messageTask.SendSms(Customer.Phone.SanitizePhoneNumber());
            }
            else
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Simulator Not Supported", 
                        Message = "Messaging is not supported in the iOS simulator.",
                        Cancel = "OK"
                    });
            }
        }

        Command _EmailCommand;

        /// <summary>
        /// Command to email customer
        /// </summary>
        public Command EmailCommand
        {
            get
            {
                return _EmailCommand ??
                (_EmailCommand = new Command(ExecuteEmailCommandCommand));
            }
        }

        void ExecuteEmailCommandCommand()
        {
            if (String.IsNullOrWhiteSpace(Customer.Email))
                return;

            if (_CapabilityService.CanSendEmail)
            {
                var emailTask = MessagingPlugin.EmailMessenger;
                if (emailTask.CanSendEmail)
                    emailTask.SendEmail(Customer.Email);
            }
            else
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Simulator Not Supported", 
                        Message = "Email composition is not supported in the iOS simulator.",
                        Cancel = "OK"
                    });
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
            var position = await GetPosition();

            var pin = new Pin() { Position = position };

            await CrossExternalMaps.Current.NavigateTo(pin.Label, pin.Position.Latitude, pin.Position.Longitude, NavigationType.Driving);
        }

        public void SetupMap()
        {
            if (HasAddress)
            {
                MessagingService.Current.SendMessage(MessageKeys.SetupMap);
            }
        }

        void DisplayGeocodingError()
        {
            MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                {
                    Title = "Geocoding Error", 
                    Message = "Please make sure the address is valid.",
                    Cancel = "OK"
                });

            IsBusy = false;
        }

        public async Task<Position> GetPosition()
        {
            if (!HasAddress)
                return new Position(0, 0);

            IsBusy = true;

            Position p;

            p = (await _Geocoder.GetPositionsForAddressAsync(Customer.AddressString)).FirstOrDefault();

            // The Android geocoder (the underlying implementation in Android itself) fails with some addresses unless they're rounded to the hundreds.
            // So, this deals with that edge case.
            if (p.Latitude == 0 && p.Longitude == 0 && AddressBeginsWithNumber(Customer.AddressString))
            {
                var roundedAddress = GetAddressWithRoundedStreetNumber(Customer.AddressString);

                p = (await _Geocoder.GetPositionsForAddressAsync(roundedAddress)).FirstOrDefault();
            }

            IsBusy = false;

            return p;
        }

        /// <summary>
        /// Subscribes to "SaveCustomer" messages
        /// </summary>
        void SubscribeToSaveCustomerMessages()
        {
            // This subscribes to the "SaveCustomer" message
            MessagingService.Current.Subscribe<Customer>(MessageKeys.SaveCustomer, (service, customer) =>
                {
                    Customer = customer;

                    // address has been updated, so we should update the map
                    if (Customer.AddressString != _AddressString)
                    {
                        MessagingService.Current.SendMessage<Customer>(MessageKeys.CustomerLocationUpdated, this.Customer);

                        _AddressString = Customer.AddressString;
                    }
                });
        }

        /// <summary>
        /// Subscribes to "CustomerLocationUpdated" messages
        /// </summary>
        void SubscribeToCustomerLocationUpdatedMessages()
        {
            // update the map when receiving the CustomerLocationUpdated message
            MessagingService.Current.Subscribe<Customer>(MessageKeys.CustomerLocationUpdated, (service, customer) =>
                {
                    OnPropertyChanged("HasAddress");

                    SetupMap();
                });
        }

        static bool AddressBeginsWithNumber(string address)
        {
            return !String.IsNullOrWhiteSpace(address) && Char.IsDigit(address.ToCharArray().First());
        }

        static string GetAddressWithRoundedStreetNumber(string address)
        {
            var endingIndex = GetEndingIndexOfNumericPortionOfAddress(address);

            if (endingIndex == 0)
                return address;

            int originalNumber = 0;
            int roundedNumber = 0;

            Int32.TryParse(address.Substring(0, endingIndex + 1), out originalNumber);

            if (originalNumber == 0)
                return address;

            roundedNumber = originalNumber.RoundToLowestHundreds();

            return address.Replace(originalNumber.ToString(), roundedNumber.ToString());
        }

        static int GetEndingIndexOfNumericPortionOfAddress(string address)
        {
            int endingIndex = 0;

            for (int i = 0; i < address.Length; i++)
            {
                if (Char.IsDigit(address[i]))
                    endingIndex = i;
                else
                    break;
            }

            return endingIndex;
        }
    }
}

