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
    public partial class OTPVerificationPage : ContentPage
    {
        public bool LoginAllowed = false;

        private readonly AudioRecorderService audioRecorderService = new AudioRecorderService();

        private readonly AudioPlayer audioPlayer = new AudioPlayer();

        public OTPVerificationPage()
        {
            InitializeComponent();
        }

        private string otp_number = "725638";

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            var file = await MediaPicker.PickPhotoAsync();

            if (file == null)
                return;

            requestData fileDetails = new requestData();

            var file1 = await file.OpenReadAsync();

            MemoryStream ms = new MemoryStream();
            await file1.CopyToAsync(ms);
            byte[] byteArray = ms.ToArray();

            fileDetails.Data = byteArray;

            string jsondata = JsonConvert.SerializeObject(fileDetails);
            StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync("https://videokycdev.azurewebsites.net/azure/api/read", content);

            var res = response.Content.ReadAsStringAsync();
            //StatusLabel.Text = response.Content.ReadAsStringAsync().Result;

            //var statuscode = response.StatusCode;
            //var statusstring = response.StatusCode.ToString();
            var value = response.Content.ReadAsStringAsync().Result;

            if(value != otp_number)
            {
                await DisplayAlert("Failure", "OTP did not match. Try again.", "OK");
            }
            else
            {
                await DisplayAlert("Success", "You can now proceed", "OK");
                LoginAllowed = true;
            }
        }

        async void Button_Clicked_1(System.Object sender, System.EventArgs e)
        {
            if (!MediaPicker.IsCaptureSupported)
            {
                await DisplayAlert("Not supported", "", "OK");
                return;
            }

            var file = await MediaPicker.CapturePhotoAsync();

            if (file == null)
                return;

            requestData fileDetails = new requestData();

            var file1 = await file.OpenReadAsync();

            MemoryStream ms = new MemoryStream();
            await file1.CopyToAsync(ms);
            byte[] byteArray = ms.ToArray();

            fileDetails.Data = byteArray;

            string jsondata = JsonConvert.SerializeObject(fileDetails);
            StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync("https://videokycdev.azurewebsites.net/azure/api/read", content);

            //StatusLabel.Text = response.Content.ReadAsStringAsync().Result;

            //var statuscode = response.StatusCode;
            //var statusstring = response.StatusCode.ToString();
            var value = response.Content.ReadAsStringAsync().Result;

            if (value != otp_number)
            {
                await DisplayAlert("Failure", "OTP did not match. Try again.", "OK");
            }
            else
            {
                await DisplayAlert("Success", "You can now proceed", "OK");
                LoginAllowed = true;
            }
        }

        async void Button_Clicked_2(System.Object sender, System.EventArgs e)
        {
            var status = await Permissions.RequestAsync<Permissions.Microphone>();

            if (status != PermissionStatus.Granted)
                return;

            if (audioRecorderService.IsRecording)
            {
                await audioRecorderService.StopRecording();

                var file = audioRecorderService.GetAudioFileStream();

                var x = audioRecorderService.GetAudioFilePath();

                if (file == null)
                    return;

                requestData fileDetails = new requestData();

                MemoryStream ms = new MemoryStream();
                await file.CopyToAsync(ms);
                byte[] byteArray = ms.ToArray();

                fileDetails.Data = byteArray;

                string jsondata = JsonConvert.SerializeObject(fileDetails);
                StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");

                var httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync("https://videokycdev.azurewebsites.net/azure/api/speech", content);

                var res = response.Content.ReadAsStringAsync();
                //StatusLabel.Text = response.Content.ReadAsStringAsync().Result;

                //var statuscode = response.StatusCode;
                //var statusstring = response.StatusCode.ToString();
                var value = response.Content.ReadAsStringAsync().Result;

                if (value != otp_number)
                {
                    await DisplayAlert("Failure", "OTP did not match. Try again.", "OK");
                }
                else
                {
                    await DisplayAlert("Success", "You can now proceed", "OK");
                    LoginAllowed = true;
                }
            }
            else
            {
                await audioRecorderService.StartRecording();
            }
        }

        private async void NavigateButton_OnClicked(object sender, EventArgs e)
        {
            if (LoginAllowed)
            {
                await Navigation.PushAsync(new UploadDocumentPage());
            }
            else
            {

            }
        }
    }
}
