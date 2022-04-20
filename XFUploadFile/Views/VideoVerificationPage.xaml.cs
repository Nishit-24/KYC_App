using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.AudioRecorder;
using XFUploadFile;
using Newtonsoft.Json;
using XFUploadFile.Models;
using System.IO;

namespace XFUploadFile.Views
{
    public partial class VideoVerificationPage : ContentPage
    {
        public bool LoginAllowed = false;

        public VideoVerificationPage()
        {
            InitializeComponent();
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (!MediaPicker.IsCaptureSupported)
            {
                await DisplayAlert("Not supported", "", "OK");
                return;
            }

            await DisplayAlert("Capture Image", "", "OK");

            var file_img = await MediaPicker.CapturePhotoAsync();

            if (file_img == null)
                return; 

            requestData fileDetails = new requestData();

            var file1 = await file_img.OpenReadAsync();

            MemoryStream ms_img = new MemoryStream();
            await file1.CopyToAsync(ms_img);
            byte[] byteArray = ms_img.ToArray();

            fileDetails.imgData = byteArray;

            await DisplayAlert("Speak this while recording", "I have filled the form and understood the details", "OK");

            var file_vid = await MediaPicker.CaptureVideoAsync();

            if (file_vid == null)
                return;

            var file2 = await file_vid.OpenReadAsync();

            MemoryStream ms_vid = new MemoryStream();
            await file2.CopyToAsync(ms_vid);
            byte[] byteArray_vid = ms_vid.ToArray();

            fileDetails.Data = byteArray_vid;

            string jsondata = JsonConvert.SerializeObject(fileDetails);
            StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync("https://videokycdev.azurewebsites.net/azure/api/face", content);

            var res = response.Content.ReadAsStringAsync();
            //StatusLabel.Text = response.Content.ReadAsStringAsync().Result;

            //var statuscode = response.StatusCode;
            //var statusstring = response.StatusCode.ToString();
            var value = response.Content.ReadAsStringAsync().Result;

            if(value == "valid")
            {
                await DisplayAlert("Success", "You can now proceed", "OK");
                LoginAllowed = true;
            }
            else
            {
                await DisplayAlert("Failure", "Verification was unsuccessful. Try again." , "OK");
            }
        }

        private async void NavigateButton_OnClicked(object sender, EventArgs e)
        {
            if (LoginAllowed)
            {
                await Navigation.PushAsync(new OTPVerificationPage());
            }
            else
            {

            }
            
        }
    }
}
