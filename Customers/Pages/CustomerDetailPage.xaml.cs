using System;
using System.Collections.Generic;

using Xamarin.Forms;

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
    }
}

