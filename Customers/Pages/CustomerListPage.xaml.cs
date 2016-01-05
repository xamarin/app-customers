using System;
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

            // on Android, we use a floating action button, not 
            if (Device.OS == TargetPlatform.Android)
                ToolbarItems.Clear();

            fab.Clicked = AndroidAddButtonClicked;
        }

        void ItemTapped (object sender, ItemTappedEventArgs e)
        {
            var page = new CustomerDetailPage();

            // NOTE: you don't typically pass a Page (or view) reference to a viewmodel, but in this case we need access to it in order to potentially display an alert from within the viewmodel.
            var viewModel = new CustomerDetailViewModel((Account)e.Item) { Navigation = this.Navigation, Page = page };

            page.BindingContext = viewModel;

            Navigation.PushAsync(page);

            ((ListView)sender).SelectedItem = null;
        }

        void AndroidAddButtonClicked (object sender, EventArgs e)
        {
            Navigation.PushAsync(new CustomerDetailPage() { BindingContext = new CustomerDetailViewModel() });
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

