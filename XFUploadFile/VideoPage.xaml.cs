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

namespace XFUploadFile
{
    public partial class VideoPage : ContentPage
    {
        public VideoPage()
        {
            InitializeComponent();
        }

        async void Button_Clicked_2(System.Object sender, System.EventArgs e)
        {
            if (!MediaPicker.IsCaptureSupported)
            {
                await DisplayAlert("Not supported", "", "OK");
                return;
            }

            await DisplayAlert("Capture Image", "", "OK");
            //await DisplayAlert("test", "", "OK");
            var file_img = await MediaPicker.CapturePhotoAsync();
            //var file_img = null;
            if (file_img == null)
                return;

            //var x = file.ContentType;

            //var client = new HttpClient();
            //var content = new StringContent(
            //    JsonConvert.SerializeObject(new { stream = await file.OpenReadAsync(), filetype = file.ContentType.ToString() }));

            requestData fileDetails = new requestData();

            var file1 = await file_img.OpenReadAsync();

            MemoryStream ms_img = new MemoryStream();
            await file1.CopyToAsync(ms_img);
            byte[] byteArray = ms_img.ToArray();

            fileDetails.imgData = byteArray;

            await DisplayAlert("Capture Video", "", "OK");

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


            //var content = new MultipartFormDataContent();
            //content.Add(new StreamContent(await file.OpenReadAsync()), "file", file.FileName);

            //var httpClient = new HttpClient();
            //var response = await httpClient.PostAsync("http://192.168.29.187:3276/videofile", content);
            //192.168.29.187
            //StatusLabel.Text = response.StatusCode.ToString();

            var res = response.Content.ReadAsStringAsync();
            StatusLabel.Text = response.Content.ReadAsStringAsync().Result;

            var statuscode = response.StatusCode;
            var statusstring = response.StatusCode.ToString();
            var value = response.Content.ReadAsStringAsync().Result;

            if (value != "-1")
            {
                await DisplayAlert("Result", response.Content.ReadAsStringAsync().Result, "OK");
                //Navigation.PushAsync(new ImagePage());
            }
            else
            {
                await DisplayAlert("Error", "You have written wrong OTP. Please write again", "OK");
            }
        }

        /*async void Button_Clicked_3(System.Object sender, System.EventArgs e)
        {
            var file = await MediaPicker.CaptureVideoAsync();

            if (file == null)
                return;

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(await file.OpenReadAsync()), "file", file.FileName);

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("http://192.168.29.187:3276/videofile", content);
            //192.168.29.187
            StatusLabel.Text = response.StatusCode.ToString();
        }

        async void Button_Clicked_5(System.Object sender, System.EventArgs e)
        {
            var file = await MediaPicker.CaptureVideoAsync();

            if (file == null)
                return;

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(await file.OpenReadAsync()), "file", file.FileName);

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("http://192.168.29.187:3276/videofile", content);
            //192.168.29.187
            StatusLabel.Text = response.StatusCode.ToString();
        }*/
    }
}
