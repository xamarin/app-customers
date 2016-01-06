using Xamarin.Forms;

namespace Customers
{
    public partial class CustomerListPage : ContentPage
    {
        protected CustomerListViewModel ViewModel
        {
            get { return BindingContext as CustomerListViewModel; }
        }

        public CustomerListPage()
        {
            InitializeComponent();
        }

        void ItemTapped (object sender, ItemTappedEventArgs e)
        {
            var page = new CustomerDetailPage();

            // NOTE: you don't typically pass a Page (or view) reference to a viewmodel, but in this case we need access to it in order to potentially display an alert from within the viewmodel.
            var viewModel = new CustomerDetailViewModel((Customer)e.Item) { Navigation = this.Navigation, Page = page };

            page.BindingContext = viewModel;

            Navigation.PushAsync(page);

            ((ListView)sender).SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (ViewModel.IsInitialized)
            {
                return;
            }
            ViewModel.LoadCustomersCommand.Execute(null);
            ViewModel.IsInitialized = true;
        }
    }
}

