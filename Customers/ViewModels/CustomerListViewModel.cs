using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using System;
using Plugin.Messaging;
using System.Linq;

namespace Customers
{
    public class CustomerListViewModel : BaseViewModel
    {
        public CustomerListViewModel()
        {
            _CapabilityService = DependencyService.Get<ICapabilityService>();

            DataSource = new CustomerDataSource();

            SubscribeToSaveCustomerMessages();

            SubscribeToDeleteCustomerMessages();

            NeedsRefresh = true;

            Customers = new ObservableCollection<Customer>();
        }

        // this is just a utility service that we're using in this demo app to mitigate some limitations of the iOS simulator
        ICapabilityService _CapabilityService;

        readonly IDataSource<Customer> DataSource;

        ObservableCollection<Customer> _Customers;

        Command _LoadCustomersCommand;

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
            IsBusy = true;
            LoadCustomersCommand.ChangeCanExecute();

            Customers = new ObservableCollection<Customer>(await DataSource.GetItems(0, 1000));

            NeedsRefresh = false;
            IsBusy = false;
            LoadCustomersCommand.ChangeCanExecute(); 
        }

        /// <summary>
        /// Command to create new customer
        /// </summary>
        public Command NewCustomerCommand
        {
            get
            {
                return _NewCustomerCommand ??
                (_NewCustomerCommand = new Command(async () =>
                        await ExecuteNewCustomerCommand()));
            }
        }

        async Task ExecuteNewCustomerCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var page = new CustomerEditPage();

            var viewModel = new CustomerDetailViewModel() { Navigation = this.Navigation, Page = page };

            page.BindingContext = viewModel;

            await Navigation.PushAsync(page);

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
                    (_DialNumberCommand = new Command(async (parameter) =>
                        await ExecuteDialNumberCommand((string)parameter)));
            }
        }

        async Task ExecuteDialNumberCommand(string customerId)
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
                await Page.DisplayAlert(
                    title: "Simulator Not Supported", 
                    message: "Phone calls are not supported in the iOS simulator.",
                    cancel: "OK");
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
                    (_MessageNumberCommand = new Command(async (parameter) =>
                        await ExecuteMessageNumberCommand((string)parameter)));
            }
        }

        async Task ExecuteMessageNumberCommand(string customerId)
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
                await Page.DisplayAlert(
                    title: "Simulator Not Supported", 
                    message: "Messaging is not supported in the iOS simulator.",
                    cancel: "OK");
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
                    (_EmailCommand = new Command(async (parameter) =>
                        await ExecuteEmailCommandCommand((string)parameter)));
            }
        }

        async Task ExecuteEmailCommandCommand(string customerId)
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
                await Page.DisplayAlert(
                    title: "Simulator Not Supported", 
                    message: "Email composition is not supported in the iOS simulator.",
                    cancel: "OK");
            }
        }

        /// <summary>
        /// Subscribes to "SaveCustomer" messages from MessagingCenter.
        /// </summary>
        void SubscribeToSaveCustomerMessages()
        {
            // This subscribes to the "SaveCustomer" message, and then inserts or updates the customer accordingly
            MessagingCenter.Subscribe<Customer>(this, "SaveCustomer", async (customer) =>
                {
                    IsBusy = true;

                    if (string.IsNullOrWhiteSpace(customer.Id))
                    {
                        customer.Id = Guid.NewGuid().ToString();
                        customer.PhotoUrl = "placeholderProfileImage";
                    }

                    await DataSource.SaveItem(customer);

                    NeedsRefresh = true;

                    IsBusy = false;
                });
        }

        /// <summary>
        /// Subscribes to "DeleteCustomer" messages from MessagingCenter.
        /// </summary>
        void SubscribeToDeleteCustomerMessages()
        {
            // This subscribes to the "DeleteCustomer" message, and then deletes the customer accordingly
            MessagingCenter.Subscribe<Customer>(this, "DeleteCustomer", async (customer) =>
                {
                    IsBusy = true;

                    await DataSource.DeleteItem(customer.Id);

                    NeedsRefresh = true;

                    IsBusy = false;
                });
        }
    }
}

