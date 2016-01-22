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

            // on Android, we use a floating action button
            if (Device.OS == TargetPlatform.Android)
                ToolbarItems.Clear();

            fab.Clicked = AndroidAddButtonClicked;
        }

        /// <summary>
        /// The action to take when a list item is tapped.
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="e">The ItemTappedEventArgs</param>
        void ItemTapped (object sender, ItemTappedEventArgs e)
        {
            Navigation.PushAsync(new CustomerDetailPage() { BindingContext = new CustomerDetailViewModel((Customer)e.Item) });

            ((ListView)sender).SelectedItem = null;
        }

        /// <summary>
        /// The action to take when the + button is clicked on Android.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The EventArgs</param>
        void AndroidAddButtonClicked (object sender, EventArgs e)
        {
            Navigation.PushAsync(new CustomerEditPage() { BindingContext = new CustomerDetailViewModel(new Customer()) });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.LoadCustomersCommand.Execute(null);
        }
    }
}

