using System;
using FormsToolkit;
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
            // send message to navigate to detail page
            MessagingService.Current.SendMessage<CustomerDetailViewModel>(MessageKeys.NavigateToDetailPage, new CustomerDetailViewModel((Customer)e.Item));

            ((ListView)sender).SelectedItem = null;
        }

        void AndroidAddButtonClicked (object sender, EventArgs e)
        {
            // send message to navigate to edit page (new customer)
            MessagingService.Current.SendMessage(MessageKeys.NavigateToEditPage);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.LoadCustomersCommand.Execute(null);

            // subscribe to messages to navigate to detail page
            MessagingService.Current.Subscribe<CustomerDetailViewModel>(MessageKeys.NavigateToDetailPage, async (service, viewmodel) =>
                await Navigation.PushAsync(new CustomerDetailPage() { BindingContext = viewmodel }));

            // subscribe to messages to navigate to edit page (new customer)
            MessagingService.Current.Subscribe(MessageKeys.NavigateToEditPage, async (service) =>
                await Navigation.PushAsync(new CustomerEditPage() { BindingContext = new CustomerDetailViewModel(new Customer()) }));

            // subscribe to messages to navigate to edit page (existing customer)
            MessagingService.Current.Subscribe<CustomerDetailViewModel>(MessageKeys.NavigateToEditPage, async (service, viewmodel) =>
                await Navigation.PushAsync(new CustomerEditPage() { BindingContext = viewmodel }));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // unsubscribe from messages to navigate to detail page
            MessagingService.Current.Unsubscribe<CustomerDetailViewModel>(MessageKeys.NavigateToDetailPage);

            // unsubscribe from messages to navigate to edit page (new customer)
            MessagingService.Current.Unsubscribe(MessageKeys.NavigateToEditPage);

            // unsubscribe from messages to navigate to edit page (existing customer)
            MessagingService.Current.Unsubscribe<CustomerDetailViewModel>(MessageKeys.NavigateToEditPage);
        }
    }
}

