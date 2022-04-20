using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace XFUploadFile.Views
{
    public partial class CustomerDetailsPage : ContentPage
    {
        public CustomerDetailsPage()
        {
            InitializeComponent();
        }

        private async void NavigateButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductDetailsPage());
        }
    }
}
