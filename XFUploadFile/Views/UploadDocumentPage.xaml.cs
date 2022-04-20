using System;
using System.Collections.Generic;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Net.Http;
using Xamarin.Essentials;
using XFUploadFile;
using Newtonsoft.Json;
using XFUploadFile.Models;

namespace XFUploadFile.Views
{
    public partial class UploadDocumentPage : ContentPage
    {
        public bool LoginAllowed = false;

        public UploadDocumentPage()
        {
            InitializeComponent();
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            var file = await FilePicker.PickAsync();

            if (file == null)
                return;

            requestData fileDetails = new requestData();

            var file1 = await file.OpenReadAsync();

            var file2 = file.GetType();

            //fileDetails.filetypeimage = "jpg";

            MemoryStream ms = new MemoryStream();
            await file1.CopyToAsync(ms);
            byte[] byteArray = ms.ToArray();

            fileDetails.Data = byteArray;

            string jsondata = JsonConvert.SerializeObject(fileDetails);
            StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync("https://videokycdev.azurewebsites.net/azure/api/read", content);

            //var res = response.Content.ReadAsStringAsync();
            //StatusLabel.Text = response.Content.ReadAsStringAsync().Result;

            //var statuscode = response.StatusCode;
            //var statusstring = response.StatusCode.ToString();
            var value = response.Content.ReadAsStringAsync().Result;

            if (value == "")
            {
                await DisplayAlert("Failure", "Please upload the document again", "OK");
            }
            else
            {
                await DisplayAlert("Success", "You can now proceed", "OK");
                LoginAllowed = true;
            }
        }

        //private MediaFile _mediaFile;
        //public string URL { get; set; }
        //bool image_uploaded = false;

        ////Picture choose from device
        //private async void Button_Clicked(object sender, EventArgs e)
        //{
        //    await CrossMedia.Current.Initialize();
        //    if (!CrossMedia.Current.IsPickPhotoSupported)
        //    {
        //        await DisplayAlert("Error", "This is not support on your device.", "OK");
        //        return;
        //    }
        //    else
        //    {
        //        var mediaOption = new PickMediaOptions()
        //        {
        //            PhotoSize = PhotoSize.Medium
        //        };
        //        _mediaFile = await CrossMedia.Current.PickPhotoAsync();
        //        imageView.Source = ImageSource.FromStream(() => _mediaFile.GetStream());
        //        UploadedUrl.Text = "Image Text";
        //    }
        //}

        //Upload picture button
        //private async void Button_Clicked_1(object sender, EventArgs e)
        //{
        //    if (_mediaFile == null)
        //    {
        //        await DisplayAlert("Error", "There was an error when trying to get your image.", "OK");
        //        return;
        //    }
        //    else
        //    {
        //        UploadImage(_mediaFile.GetStream());
        //    }
        //}

        ////Upload to blob function
        //private async void UploadImage(Stream stream)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    stream.CopyTo(ms);
        //    byte[] byteArray = ms.ToArray();

        //    Busy();
        //    var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=documentproof;AccountKey=9R/UULNA/furiFQloMkmctFBUus7SkvylnsdmbP12BKwS3/oi2XAu6555MykyfugvE4B+9tMo6UdyRLQ36lPHA==;EndpointSuffix=core.windows.net");
        //    var client = account.CreateCloudBlobClient();
        //    var container = client.GetContainerReference("documents");
        //    await container.CreateIfNotExistsAsync();
        //    var name = Guid.NewGuid().ToString();
        //    var blockBlob = container.GetBlockBlobReference($"{name}.png");
        //    await blockBlob.UploadFromStreamAsync(stream);
        //    //URL = blockBlob.Uri.OriginalString;
        //    //UploadedUrl.Text = URL;
        //    NotBusy();
        //    image_uploaded = true;
        //    await DisplayAlert("Uploaded", "Image uploaded to Blob Storage Successfully!", "OK");
        //}

        //public void Busy()
        //{
        //    uploadIndicator.IsVisible = true;
        //    uploadIndicator.IsRunning = true;
        //    btnSelectPic.IsEnabled = false;
        //    btnUpload.IsEnabled = false;
        //}

        //public void NotBusy()
        //{
        //    uploadIndicator.IsVisible = false;
        //    uploadIndicator.IsRunning = false;
        //    btnSelectPic.IsEnabled = true;
        //    btnUpload.IsEnabled = true;
        //}

        private async void NavigateButton_OnClicked(object sender, EventArgs e)
        {
            if (LoginAllowed)
            {
                await Navigation.PushAsync(new SuccessPage());
            }
            else
            {

            }
        }
    }
}
