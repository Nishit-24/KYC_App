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
    public partial class MainPage : ContentPage
    {
        private readonly AudioRecorderService audioRecorderService = new AudioRecorderService();

        private readonly AudioPlayer audioPlayer = new AudioPlayer();

        public MainPage()
        {
            InitializeComponent();
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            var file = await MediaPicker.PickPhotoAsync();

            if (file == null)
                return;

            //var x = file.ContentType;

            //var client = new HttpClient();
            //var content = new StringContent(
            //    JsonConvert.SerializeObject(new { stream = await file.OpenReadAsync(), filetype = file.ContentType.ToString() }));

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

            //var content = new MultipartFormDataContent();
            //content.Add(new StreamContent(await file.OpenReadAsync()), "file", file.FileName);

            //var httpClient = new HttpClient();
            //var response = await httpClient.PostAsync("https://videokycdev.azurewebsites.net/azure/api/read", content);

            //192.168.29.187
            //string res = JsonConvert.DeserializeObject(response);

            //var x = response.Content.ReadAsStringAsync().Result;
            //var data = JsonConvert.DeserializeObject<result>(x);

            var res = response.Content.ReadAsStringAsync();
            StatusLabel.Text = response.Content.ReadAsStringAsync().Result;

            var statuscode = response.StatusCode;
            var statusstring = response.StatusCode.ToString();
            var value = response.Content.ReadAsStringAsync().Result;

            //if (value != "-1")
            //{
            //    await DisplayAlert("You have wrote", response.Content.ReadAsStringAsync().Result , "OK");
            //    //Navigation.PushAsync(new VideoPage());
            //}
            //else
            //{
            //    await DisplayAlert("Error", "You have written wrong OTP. Please write again", "OK");
            //}
            //response.StatusCode.ToString();
        }

        async void Button_Clicked_1(System.Object sender, System.EventArgs e)
        {
            //if (result != null)
            //{
            //    var stream = await result.OpenReadAsync();
            //    MemoryStream ms = new MemoryStream();
            //    stream.CopyTo(ms);
            //    byte[] byteArray = ms.ToArray();
            //    resultImage.Source = ImageSource.FromStream(() => stream);
            //}
            //if (result != null)
            //{
            //    var stream = await result.OpenReadAsync();
            //    resultImage.Source = ImageSource.FromStream(() => stream);
            //}

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

            //var content = new MultipartFormDataContent();
            //content.Add(new StreamContent(await file.OpenReadAsync()), "file", file.FileName);

            //var httpClient = new HttpClient();
            //var response = await httpClient.PostAsync("http://192.168.29.187:3276/UploadFile", content);
            //192.168.29.187
            StatusLabel.Text = response.Content.ReadAsStringAsync().Result;

            var statuscode = response.StatusCode;
            var statusstring = response.StatusCode.ToString();
            var value = response.Content.ReadAsStringAsync().Result;

            //if (value != "-1")
            //{
            //    await DisplayAlert("You have wrote", response.Content.ReadAsStringAsync().Result, "OK");
            //    //Navigation.PushAsync(new VideoPage());
            //}
            //else
            //{
            //    await DisplayAlert("Error", "You have written wrong OTP. Please write again", "OK");
            //}

        }

        async void Button_Clicked_4(System.Object sender, System.EventArgs e)
        {
            // This was not in the video, we need to ask permission
            // for the microphone to make it work for Android, see https://youtu.be/uBdX54sTCP0
            //await DisplayAlert("Note", "After you speak, press the recording button again to stop", "OK");

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

               // var file1 = await file.OpenReadAsync();

                MemoryStream ms = new MemoryStream();
                await file.CopyToAsync(ms);
                byte[] byteArray = ms.ToArray();

                fileDetails.Data = byteArray;

                string jsondata = JsonConvert.SerializeObject(fileDetails);
                StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");

                var httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync("https://videokycdev.azurewebsites.net/azure/api/speech", content);

                //var content = new MultipartFormDataContent();
                //content.Add(new StreamContent(file), "file", "audiofile");

                //audioPlayer.Play(audioRecorderService.GetAudioFilePath());

                //var httpClient = new HttpClient();
                //var response = await httpClient.PostAsync("http://192.168.29.187:3276/audiofile", content);
                //192.168.29.187
                //StatusLabel.Text = response.StatusCode.ToString();
                var res = response.Content.ReadAsStringAsync();
                StatusLabel.Text = response.Content.ReadAsStringAsync().Result;
                //audioPlayer.Play(audioRecorderService.GetAudioFilePath());

                var statuscode = response.StatusCode;
                var statusstring = response.StatusCode.ToString();
                var value = response.Content.ReadAsStringAsync().Result;

                
                //if (value != "-1")
                //{
                //    await DisplayAlert("You have spoke", response.Content.ReadAsStringAsync().Result, "OK");
                //    //Navigation.PushAsync(new VideoPage());
                //}
                //else
                //{
                //    await DisplayAlert("Error", "You have spoke wrong OTP. Please speak again", "OK");
                //}
            }
            else
            {
                await audioRecorderService.StartRecording();
            }
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

            //if (value != "-1")
            //{
            //    await DisplayAlert("Result", response.Content.ReadAsStringAsync().Result, "OK");
            //    //Navigation.PushAsync(new ImagePage());
            //}
            //else
            //{
            //    await DisplayAlert("Error", "You have written wrong OTP. Please write again", "OK");
            //}
        }

        async void Button_Clicked_6(System.Object sender, System.EventArgs e)
        {
            await DisplayAlert("Note", "1. Upload image having some Numeric value from mobile to convert it to text \n " +
                "2. Capture image having some numeric value from mobile camera to convert it to text \n " +
                "3. Press Start Recording button, speak a number, press Stop Recording button to convert audio to text \n " +
                "4. Capture Image first, then capture video to identify user related details \n\n " +
                "Results of each process will be displayed through a pop-up in some time. ", "OK");
        }
    }
}

