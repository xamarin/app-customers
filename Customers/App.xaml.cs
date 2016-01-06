using Xamarin.Forms;

namespace Customers
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            navPage.BarTextColor = Color.White;

            customerListPage.BindingContext = new CustomerListViewModel() { Navigation = navPage.Navigation }; // set the context for the customer list page to a new instance of CustomerListViewModel, giving it the same Navigation instance as the navigation page.
        }
    }
}

