using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FormsToolkit;
using Plugin.Messaging;
using Xamarin.Forms;

namespace Customers
{
    public class CustomerListViewModel : BaseNavigationViewModel
    {
        public CustomerListViewModel()
        {
            _CapabilityService = DependencyService.Get<ICapabilityService>();

            DataSource = new CustomerDataSource();

            SubscribeToSaveCustomerMessages();

            SubscribeToDeleteCustomerMessages();
        }

        // this is just a utility service that we're using in this demo app to mitigate some limitations of the iOS simulator
        ICapabilityService _CapabilityService;

        readonly IDataSource<Customer> DataSource;

        ObservableCollection<Customer> _Customers;

        Command _LoadCustomersCommand;

        Command _RefreshCustomersCommand;

        Command _NewCustomerCommand;

        public ObservableCollection<Customer> Customers
        {
            get { return _Customers; }
            set
            {
                _Customers = value;
                OnPropertyChanged("Customers");
            }
        }

        /// <summary>
        /// Command to load customers
        /// </summary>
        public Command LoadCustomersCommand
        {
            get { return _LoadCustomersCommand ?? (_LoadCustomersCommand = new Command(async () => await ExecuteLoadCustomersCommand())); }
        }

        async Task ExecuteLoadCustomersCommand()
        {
            if (Customers == null)
            {
                LoadCustomersCommand.ChangeCanExecute();

                await FetchCustomers();

                LoadCustomersCommand.ChangeCanExecute();
            }
        }

        public Command RefreshCustomersCommand
        {
            get { 
                return _RefreshCustomersCommand ?? (_RefreshCustomersCommand = new Command(async () => await ExecuteRefreshCustomersCommandCommand())); 
            }
        }

        async Task ExecuteRefreshCustomersCommandCommand()
        {
            RefreshCustomersCommand.ChangeCanExecute();

            await FetchCustomers();

            RefreshCustomersCommand.ChangeCanExecute();
        }

        async Task FetchCustomers()
        {
            IsBusy = true;

            Customers = new ObservableCollection<Customer>(await DataSource.GetItems(0, 1000));

            IsBusy = false;
        }

        /// <summary>
        /// Command to create new customer
        /// </summary>
        public Command NewCustomerCommand
        {
            get
            {
                return _NewCustomerCommand ??
                    (_NewCustomerCommand = new Command(async () => await ExecuteNewCustomerCommand()));
            }
        }

        async Task ExecuteNewCustomerCommand()
        {
            await PushAsync(new CustomerEditPage() { BindingContext = new CustomerDetailViewModel(new Customer()) });
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
                (_DialNumberCommand = new Command((parameter) =>
                        ExecuteDialNumberCommand((string)parameter)));
            }
        }

        void ExecuteDialNumberCommand(string customerId)
        {
            if (String.IsNullOrWhiteSpace(customerId))
                return;

            var customer = _Customers.SingleOrDefault(c => c.Id == customerId);

            if (customer == null)
                return;

            if (_CapabilityService.CanMakeCalls)
            {
                var phoneCallTask = MessagingPlugin.PhoneDialer;
                if (phoneCallTask.CanMakePhoneCall)
                    phoneCallTask.MakePhoneCall(customer.Phone.SanitizePhoneNumber());
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
                (_MessageNumberCommand = new Command((parameter) =>
                        ExecuteMessageNumberCommand((string)parameter)));
            }
        }

        void ExecuteMessageNumberCommand(string customerId)
        {
            if (String.IsNullOrWhiteSpace(customerId))
                return;

            var customer = _Customers.SingleOrDefault(c => c.Id == customerId);

            if (customer == null)
                return;     

            if (_CapabilityService.CanSendMessages)
            {
                var messageTask = MessagingPlugin.SmsMessenger;
                if (messageTask.CanSendSms)
                    messageTask.SendSms(customer.Phone.SanitizePhoneNumber());
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
                (_EmailCommand = new Command((parameter) =>
                        ExecuteEmailCommandCommand((string)parameter)));
            }
        }

        void ExecuteEmailCommandCommand(string customerId)
        {
            if (String.IsNullOrWhiteSpace(customerId))
                return;

            var customer = _Customers.SingleOrDefault(c => c.Id == customerId);

            if (customer == null)
                return;

            if (_CapabilityService.CanSendEmail)
            {
                var emailTask = MessagingPlugin.EmailMessenger;
                if (emailTask.CanSendEmail)
                    emailTask.SendEmail(customer.Email);
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

        /// <summary>
        /// Subscribes to "SaveCustomer" messages
        /// </summary>
        void SubscribeToSaveCustomerMessages()
        {
            // This subscribes to the "SaveCustomer" message, and then inserts or updates the customer accordingly
            MessagingService.Current.Subscribe<Customer>(MessageKeys.SaveCustomer, async (service, customer) =>
                {
                    IsBusy = true;

                    await DataSource.SaveItem(customer);

                    await FetchCustomers();

                    IsBusy = false;
                });
        }

        /// <summary>
        /// Subscribes to "DeleteCustomer" messages
        /// </summary>
        void SubscribeToDeleteCustomerMessages()
        {
            // This subscribes to the "DeleteCustomer" message, and then deletes the customer accordingly
            MessagingService.Current.Subscribe<Customer>(MessageKeys.DeleteCustomer, async (service, customer) =>
                {
                    IsBusy = true;

                    await DataSource.DeleteItem(customer.Id);

                    await FetchCustomers();

                    IsBusy = false;
                });
        }
    }
}

