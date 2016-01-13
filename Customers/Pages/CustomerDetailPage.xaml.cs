using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;

namespace Customers
{
    public partial class CustomerDetailPage : ContentPage
    {
        protected CustomerDetailViewModel ViewModel
        {
            get { return BindingContext as CustomerDetailViewModel; }
        }

        public CustomerDetailPage()
        {
            InitializeComponent();
        }

        public Map Map
        {
            get { return _Map; }
        }

        async protected override void OnAppearing()
        {
            base.OnAppearing();

            await ViewModel.SetupMap();
        }
    }
}

