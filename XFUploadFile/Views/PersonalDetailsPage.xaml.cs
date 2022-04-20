using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace XFUploadFile.Views
{
    public partial class PersonalDetailsPage : ContentPage
    {
        public PersonalDetailsPage()
        {
            InitializeComponent();
        }

        private async void NavigateButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LifeStylePage());
        }
    }
}
