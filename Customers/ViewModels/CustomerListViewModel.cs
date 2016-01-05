using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Xamarin.Forms;

namespace Customers
{
    public class CustomerListViewModel : BaseViewModel
    {
        public CustomerListViewModel()
        {
            SubscribeToSaveCustomerMessages();

            SubscribeToDeleteCustomerMessages();
        }

        MobileServiceSQLiteStore _Store;

        ObservableCollection<Account> _Accounts;

        Command _LoadCustomersCommand;

        Command _CustomersRefreshCommand;

        Command _NewCustomerCommand;

        bool _IsSeeded;

        static IMobileServiceSyncTable<Account> _AccountTable;

        public static IMobileServiceSyncTable<Account> AccountTable { get { return _AccountTable; } }

        public static IMobileServiceClient MobileServiceClient { get { return _LazyMobileServiceClient.Value; } }

        private static Lazy<MobileServiceClient> _LazyMobileServiceClient = 
            new Lazy<MobileServiceClient>(() =>
                new MobileServiceClient("http://xamarincrmservice.azurewebsites.net/"));

        public async Task SeedLocalDataAsync()
        {
            await InitializeLocalDataStore();

            await FetchRemoteAccounts();

            _IsSeeded = true;
        }

        async Task InitializeLocalDataStore()
        {
            _Store = new MobileServiceSQLiteStore("syncstore.db");

            _Store.DefineTable<Account>();

            await MobileServiceClient.SyncContext.InitializeAsync(_Store, new CustomSyncHandler());

            _AccountTable = MobileServiceClient.GetSyncTable<Account>();
        }

        /// <summary>
        /// Populates the public Accounts property.
        /// </summary>
        /// <returns>A Task.</returns>
        async Task FetchLocalAccounts()
        {
            Accounts = new ObservableCollection<Account>(await _AccountTable.OrderBy(x => x.LastName).ToEnumerableAsync());
        }

        /// <summary>
        /// Fetchs the accounts from the remote table into the local table.
        /// </summary>
        /// <returns>A Task.</returns>
        async Task FetchRemoteAccounts()
        {
            await _AccountTable.PullAsync("syncAccounts", _AccountTable.Where(x => !x.Deleted));
        }

        public ObservableCollection<Account> Accounts
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

            if (!_IsSeeded)
                await SeedLocalDataAsync();

            await FetchLocalAccounts();

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

            await FetchRemoteAccounts();

            await FetchLocalAccounts();

            IsBusy = false;
        }

        // This subscribes to the "SaveCustomer" message, and then inseets or updates the customer accordingly
        void SubscribeToSaveCustomerMessages()
        {

            MessagingCenter.Subscribe<Account>(this, "SaveCustomer", async (account) =>
                {
                    if (account.Id == null)
                        await _AccountTable.InsertAsync(account);
                    else
                        await _AccountTable.UpdateAsync(account);

                    await FetchLocalAccounts();
                });
        }

        // This subscribes to the "DeleteCustomer" message, and then deletes the customer accordingly
        void SubscribeToDeleteCustomerMessages()
        {

            MessagingCenter.Subscribe<Account>(this, "DeleteCustomer", async (account) =>
                {
                    await _AccountTable.DeleteAsync(account);

                    await FetchLocalAccounts();
                });
        }
    }
}

