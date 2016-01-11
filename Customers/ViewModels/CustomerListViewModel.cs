using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Customers
{
    public class CustomerListViewModel : BaseViewModel
    {
        public CustomerListViewModel()
        {
            DataSource = new CustomerDataSource();

            SubscribeToSaveCustomerMessages();

            SubscribeToDeleteCustomerMessages();
        }

        IDataSource<Customer> DataSource;

        ObservableCollection<Customer> _Accounts;

        Command _LoadCustomersCommand;

        Command _CustomersRefreshCommand;

        Command _NewCustomerCommand;

        /// <summary>
        /// Fetchs the accounts from the remote table into the local table.
        /// </summary>
        /// <returns>A Task.</returns>
        async Task FetchCustomers()
        {
            Accounts = new ObservableCollection<Customer>(await DataSource.GetItems(0, 1000));
        }

        public ObservableCollection<Customer> Accounts
        {
            get { return _Accounts; }
            set
            {
                _Accounts = value;
                OnPropertyChanged("Accounts");
            }
        }

        /// <summary>
        /// Command to load accounts
        /// </summary>
        public Command LoadCustomersCommand
        {
            get { return _LoadCustomersCommand ?? (_LoadCustomersCommand = new Command(async () => await ExecuteLoadCustomersCommand())); }
        }

        async Task ExecuteLoadCustomersCommand()
        {
            IsBusy = true;
            LoadCustomersCommand.ChangeCanExecute();

            await FetchCustomers();

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

            var page = new CustomerDetailPage();

            var viewModel = new CustomerDetailViewModel() { Navigation = this.Navigation, Page = page };

            page.BindingContext = viewModel;

            await Navigation.PushAsync(page);

            IsBusy = false;
        }

        /// <summary>
        /// Command to fetch emote customers
        /// </summary>
        public Command CustomersRefreshCommand
        {
            get
            {
                return _CustomersRefreshCommand ??
                (_CustomersRefreshCommand = new Command(async () =>
                        await ExecuteCustomersRefreshCommand()));
            }
        }

        async Task ExecuteCustomersRefreshCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await FetchCustomers();

            IsBusy = false;
        }

        void SubscribeToSaveCustomerMessages()
        {
            // This subscribes to the "SaveCustomer" message, and then inserts or updates the customer accordingly
            MessagingCenter.Subscribe<Customer>(this, "SaveCustomer", async (customer) =>
                {
                    IsBusy = true;

                    await DataSource.SaveItem(customer);

                    await FetchCustomers();

                    IsBusy = false;
                });
        }

        void SubscribeToDeleteCustomerMessages()
        {
            // This subscribes to the "DeleteCustomer" message, and then deletes the customer accordingly
            MessagingCenter.Subscribe<Customer>(this, "DeleteCustomer", async (customer) =>
                {
                    IsBusy = true;

                    await DataSource.DeleteItem(customer.Id);

                    await FetchCustomers();

                    IsBusy = false;
                });
        }
    }
}

