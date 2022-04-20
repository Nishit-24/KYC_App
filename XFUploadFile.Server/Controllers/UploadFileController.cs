using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using XFUploadFile.Server.Models;
using System.Collections.Generic;
using OpenCvSharp;
using System.Net;
using System.Drawing;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.CognitiveServices.Vision;
using OpenCvSharp.Extensions; 

namespace XFUploadFile.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadFileController : ControllerBase
    {
        CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=storeimage24;AccountKey=/ety9iqeOFQcnXgAgDXGoa7/wSyrIApQyzrlrTsoMJcfuz5E5p9+L+wVoibhpfcGzEYk45jAllK+dEfbPByBaQ==;EndpointSuffix=core.windows.net");

        private readonly ILogger<UploadFileController> _logger;
        private readonly IWebHostEnvironment _environment;

        static string subscriptionKey = "007d8071d14748b9981776c3c656958d";
        static string endpoint = "https://ocrapptrial.cognitiveservices.azure.com/";

        string READ_TEXT_URL_IMAGE = "";

        public UploadFileController(ILogger<UploadFileController> logger,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        [HttpPost]
        public async Task<string> PostImage()
        {
            //try
            //{
            var httpRequest = HttpContext.Request;

            var res = httpRequest.Form;

            //if (httpRequest.Form.Files.Count > 0)
            //{
            foreach (var file in httpRequest.Form.Files)
            {
                //var filePath = Path.Combine(_environment.ContentRootPath, "uploads");

                //    if (!Directory.Exists(filePath))
                //        Directory.CreateDirectory(filePath);

                //using (var memoryStream = new MemoryStream())
                //{
                //    await file.CopyToAsync(memoryStream); System.IO.File.WriteAllBytes(Path.Combine(filePath, file.FileName), memoryStream.ToArray());

                await UploadToAzureAsync(file);
                //}

                string response = null;

                MemoryStream ms = new MemoryStream();
                await file.CopyToAsync(ms);
                byte[] byteArray = ms.ToArray();

                ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);

                //ocr2(byteArray);

                try
                {
                    await Task.Run(() => { response = ReadFilestream(client, byteArray).Result; }).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return ex.Message;
                }
                // ...   
                return response;

                //System.IO.File.WriteAllBytes(Path.Combine(filePath, file.FileName), byteArray);
            }
            //}
            //}
            //catch (Exception e)
            //{
            //    _logger.LogError(e, "Error");
            //    return new StatusCodeResult(500);
            //}
            return "";
            //return BadRequest();
        }

        [HttpPost]
        [Route("/audiofile")]
        public async Task<string> PostAudio()
        {
            //try
            //{
                var httpRequest = HttpContext.Request;

                var res = httpRequest.Form;

                if (httpRequest.Form.Files.Count > 0)
                {
                    foreach (var file in httpRequest.Form.Files)
                    {
                        //var filePath = Path.Combine(_environment.ContentRootPath, "uploads");

                        //if (!Directory.Exists(filePath))
                        //    Directory.CreateDirectory(filePath);

                        //using (var memoryStream = new MemoryStream())
                        //{
                        //    await file.CopyToAsync(memoryStream); System.IO.File.WriteAllBytes(Path.Combine(filePath, file.FileName), memoryStream.ToArray());

                        //    await UploadToAzureAsync(file);
                        //}

                        await UploadToAzureAsync(file);

                        MemoryStream ms = new MemoryStream();
                        await file.CopyToAsync(ms);
                        byte[] byteArray = ms.ToArray();

                        //System.IO.File.WriteAllBytes(Path.Combine(filePath, file.FileName), byteArray);

                        string response = null;

                        try
                        {
                            await Task.Run(() => { response = RecognizeSpeechAsync(byteArray, READ_TEXT_URL_IMAGE).Result; }).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            return ex.Message;
                        }
                        // ...

                        return response;

                        //return Ok();
                    }
                }
            //}
            //catch (Exception e)
            //{
            //    _logger.LogError(e, "Error");
            //    return new StatusCodeResult(500);
            //}

            //return BadRequest();
            return "not done";
        }

        [HttpPost]
        [Route("/videofile")]
        public async Task<string> PostVideo()
        {
            //try
            //{
                var httpRequest = HttpContext.Request;

                var res = httpRequest.Form;

                if (httpRequest.Form.Files.Count > 0)
                {
                    foreach (var file in httpRequest.Form.Files)
                    {
                        //var filePath = Path.Combine(_environment.ContentRootPath, "uploads");

                        //if (!Directory.Exists(filePath))
                        //    Directory.CreateDirectory(filePath);

                        //using (var memoryStream = new MemoryStream())
                        //{
                        //    await file.CopyToAsync(memoryStream); System.IO.File.WriteAllBytes(Path.Combine(filePath, file.FileName), memoryStream.ToArray());

                        //    await UploadToAzureAsync(file);
                        //}

                        MemoryStream ms = new MemoryStream();
                        await file.CopyToAsync(ms);
                        byte[] byteArray = ms.ToArray();

                        string response = null;
                        string subscriptionKey = "fd0f0a4e89364ba0b926a1ceba3aa024";
                        string endpoint = "https://facedetail.cognitiveservices.azure.com/";

                        // the Analyze method endpoint
                        string uriBase = endpoint;

                        // Image you want analyzed (add to your bin/debug/netcoreappX.X folder)
                        // For sample images, download one from here (png or jpg):
                        // https://github.com/Azure-Samples/cognitive-services-sample-data-files/tree/master/ComputerVision/Images
                        //string imageFilePath = "C:/Users/siddh/Desktop/azurecheck/1.jpg";

                        try
                        {
                            await Task.Run(() => { response = MakeDetectRequest(byteArray, subscriptionKey, uriBase).Result; }).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            return ex.Message;
                        }
                        // ...   
                        return response;
                        //System.IO.File.WriteAllBytes(Path.Combine(filePath, file.FileName), byteArray);
                    }
                }
            //}
            return "not done";

        }

        [HttpPost]
        [Route("/videofile2")]
        public async Task<string> PostVideo2()
        {
            //try
            //{
            var httpRequest = HttpContext.Request;

            var res = httpRequest.Form;

            if (httpRequest.Form.Files.Count > 0)
            {
                foreach (var file in httpRequest.Form.Files)
                {
                    //var filePath = Path.Combine(_environment.ContentRootPath, "uploads");

                    //if (!Directory.Exists(filePath))
                    //    Directory.CreateDirectory(filePath);

                    //using (var memoryStream = new MemoryStream())
                    //{
                    //    await file.CopyToAsync(memoryStream); System.IO.File.WriteAllBytes(Path.Combine(filePath, file.FileName), memoryStream.ToArray());

                    //    await UploadToAzureAsync(file);
                    //}

                    MemoryStream ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    byte[] byteArray = ms.ToArray();

                    string response1 = null;
                    string response2 = null;
                    string subscriptionKey = "fd0f0a4e89364ba0b926a1ceba3aa024";
                    string endpoint = "https://facedetail.cognitiveservices.azure.com/";
                    string response = null;
                    // the Analyze method endpoint
                    string uriBase = endpoint;
                    //string fileLocation = @"C:/Users/siddh/Desktop/azurecheck/2.mp4";
                    string fileLocation = @"/Users/nishitjagetia/Desktop/test_video.mp4";

                    System.IO.File.WriteAllBytes(fileLocation, byteArray);

                    string location = extract(fileLocation);

                    string imgpath = location + "/" + "frame1.jpg";
                    byte[] data = GetImageAsByteArray(imgpath);
                    string imgpath1 = location + "/" + "frame2.jpg";
                    byte[] data1 = GetImageAsByteArray(imgpath1);
                    //string imgpath2 = "C:/Users/siddh/Desktop/azurecheck/1.jpg";
                    //byte[] data2 = GetImageAsByteArray(imgpath2);

                    try
                    {
                        await Task.Run(() => { response1 = MakeDetectRequest(data, subscriptionKey, uriBase).Result; }).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {

                        return ex.Message;
                    }
                    try
                    {
                        await Task.Run(() => { response2 = MakeDetectRequest(data1, subscriptionKey, uriBase).Result; }).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }

                    string json1 = response1.Trim().Trim('[', ']');
                    var result1 = JsonConvert.DeserializeObject<faceresponse>(json1);
                    string json2 = response2.Trim().Trim('[', ']');
                    var result2 = JsonConvert.DeserializeObject<faceresponse>(json2);

                    try
                    {
                        await Task.Run(() => { response = VerifyPerson(result1.faceId, result2.faceId, subscriptionKey, uriBase).Result; }).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return ex.Message;
                    }
                    string json3 = response.Trim().Trim('[', ']');
                    var result = JsonConvert.DeserializeObject<verifyResponceModel>(json3);

                    if (!result.isIdentical)
                    {
                        return "the face are not identical";
                    }
                    //string[] files = Directory.GetFiles(location);
                    //foreach (string file in files)
                    // {
                    // System.IO.File.Delete(file);
                    // Console.WriteLine($"{file} is deleted.");
                    // }
                    // System.IO.File.Delete(fileLocation);
                    if (result2.faceAttributes.emotion.anger == result1.faceAttributes.emotion.anger &&
                       result2.faceAttributes.emotion.contempt == result1.faceAttributes.emotion.contempt &&
                       result2.faceAttributes.emotion.disgust == result1.faceAttributes.emotion.disgust &&
                       result2.faceAttributes.emotion.fear == result1.faceAttributes.emotion.fear &&
                       result2.faceAttributes.emotion.happiness == result1.faceAttributes.emotion.happiness &&
                       result2.faceAttributes.emotion.neutral == result1.faceAttributes.emotion.neutral &&
                       result2.faceAttributes.emotion.surprise == result1.faceAttributes.emotion.surprise &&
                       result2.faceAttributes.emotion.sadness == result1.faceAttributes.emotion.sadness)
                    {
                        return "not valid same emotion";
                    }

                    return "valid";

                    //System.IO.File.WriteAllBytes(Path.Combine(filePath, file.FileName), byteArray);
                }
            }
            //}
            return "not done";
        }

        public async Task<string> VerifyPerson(string sfaceid1, string sfaceid2, string subscriptionKey, string uriBase)
        {
            string result = "";
            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                string uri = uriBase + "face/v1.0/verify";
                Console.WriteLine(uri);
                HttpResponseMessage response;
                string operationLocation;
                // Read the contents of the specified local image
                // into a byte array.

                var value = new Dictionary<string, string>
                {
                    { "faceId1" , sfaceid1},
                    { "faceId2", sfaceid2}
                };
                string json = JsonConvert.SerializeObject(value);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                // Asynchronously call the REST API method.

                response = await client.PostAsync(uri, content);
                operationLocation = response.Headers.ToString();

                // Display the JSON response.
                //Console.WriteLine("\nResponse:\n\n{0}\n",
                //    JToken.Parse(contentString).ToString());
                // Get the JSON response.
                result = await response.Content.ReadAsStringAsync();
                return result;
                // Display the JSON response.

            }
            catch (Exception e)
            {
                result = e.Message + "this is the error";
            }

            return result;
        }

        public string extract(string videoFile)
        {
            // Specify path to MP4 video file.
            //string outpath = @"C:/Users/siddh/Desktop/azurecheck/faces/";
            string outpath = @"/Users/nishitjagetia/Desktop/faces/";
            System.IO.Directory.CreateDirectory(outpath);

            var capture = new VideoCapture(videoFile);
            var image = new Mat();
            int i = 0;

            while (capture.IsOpened())
            {
                // Read next frame in video file
                capture.Read(image);
                if (image.Empty())
                {
                    break;
                }
                // Save image to disk.
                Cv2.ImWrite(String.Format(outpath + "frame{0}.jpg", i), image);

                i++;
            }
            return outpath;
        }

        private async Task UploadToAzureAsync(IFormFile file)
        {
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            var cloudBlobContainer = cloudBlobClient.GetContainerReference("testcontainer");

            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });
            }

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.FileName);
            cloudBlockBlob.Properties.ContentType = file.ContentType;

            await cloudBlockBlob.UploadFromStreamAsync(file.OpenReadStream());

            READ_TEXT_URL_IMAGE = cloudBlockBlob.Uri.OriginalString;

            //ocr(READ_TEXT_URL_IMAGE);
        }

        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        public static async Task<string> ReadFilestream(ComputerVisionClient client, byte[] data)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("READ FILE FROM URL");
            Console.WriteLine();

            string display = null;
            Stream stream = new MemoryStream(data);

            // Read text from URL
            //var textHeaders = await client.ReadAsync(urlFile);

            var textHeaders = await client.ReadInStreamAsync(stream);


            // After the request, get the operation location (operation ID)
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);
            // </snippet_readfileurl_1>

            // <snippet_readfileurl_2>
            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;
            //Console.WriteLine($"Extracting text from URL file {Path.GetFileName(urlFile)}...");
            Console.WriteLine();
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));
            // </snippet_readfileurl_2>

            // <snippet_readfileurl_3>
            // Display the found text.
            Console.WriteLine();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    //Console.WriteLine(line.Text);
                    display += line.Text;
                }
            }

            string result = "";
            for (int i = 0; i < display.Length; i++)
            {
                if (Char.IsNumber(display[i])) result += display[i];
            }
            Console.WriteLine(result);
            int num = 1972;
            string s = num.ToString();

            if (s == result) return result;
            return "-1";
        }

        public static async Task<string> RecognizeSpeechAsync(byte[] data, string add)
        {
            string results = null;
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key // and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription("e5d6dce34c0d41aeb7a89518ee299f86", "centralindia");

            //string FilePath = "C:/Users/siddh/Downloads/nwe.wav";
            System.IO.File.WriteAllBytes(@"/Users/nishitjagetia/Desktop/test.wav", data);

            var audioconfig = AudioConfig.FromWavFileInput("/Users/nishitjagetia/Desktop/test.wav");

            // AudioConfig.FromStreamInput();
            //Microsoft.CognitiveServices.Speech.Audio.

            //PushAudioInputStream pushStream = AudioInputStream.CreatePushStream();

            //pushStream.Write(data);

            //pushStream.Close();

            //var audioconfig = AudioConfig.FromStreamInput(pushStream, );

            //pushStream.Close();
            //var audioconfig = AudioConfig.FromStreamInput();
            // Creates a speech recognizer.
            // SpeechRecognizer(SpeechConfig speechConfig, AudioConfig audioConfig);
            using (var recognizer = new SpeechRecognizer(config, audioconfig))
            {
                // Starts speech recognition, and returns after a single utterance is recognized. The end of a
                // single utterance is determined by listening for silence at the end or until a maximum of 15
                // seconds of audio is processed.  The task returns the recognition text as result. 
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                // shot recognition like command or query. 
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
                //pushStream.Write(data);
                //pushStream.Close();
                var result = await recognizer.RecognizeOnceAsync();
                
                // Checks result.
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    results = result.Text;
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    results = cancellation.Reason.ToString();

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        results = cancellation.ErrorCode.ToString() + cancellation.ErrorDetails.ToString();

                    }
                }
                
            }

            int num = 1972;
            string s = num.ToString();

            if (results == s) return results;
            return "-1";
        }

        public static async Task<string> MakeDetectRequest(byte[] byteData, string subscriptionKey, string uriBase)
        {
            string result = "";
            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                string uri = uriBase + "face/v1.0/detect?returnFaceId=true&returnFaceLandmarks=true&returnFaceAttributes=emotion";
                Console.WriteLine(uri);
                HttpResponseMessage response;
                string operationLocation;
                // Read the contents of the specified local image
                // into a byte array.

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {

                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // Asynchronously call the REST API method.
                    response = await client.PostAsync(uri, content);
                    operationLocation = response.Headers.ToString();

                    // Display the JSON response.
                    //Console.WriteLine("\nResponse:\n\n{0}\n",
                    //    JToken.Parse(contentString).ToString());
                    // Get the JSON response.
                    result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                // Display the JSON response.

            }
            catch (Exception e)
            {
                result = e.Message + "this is the error";
            }
            return result;
        }

        static async Task<string> MakeSpeechRequest(string imageFilePath, string subscriptionKey, string uriBase)
        {
            string result = "";
            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                string uri = uriBase + "face/v1.0/detect?returnFaceId=true&returnFaceLandmarks=false";
                Console.WriteLine(uri);
                HttpResponseMessage response;
                string operationLocation;
                // Read the contents of the specified local image
                // into a byte array.
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {

                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // Asynchronously call the REST API method.
                    response = await client.PostAsync(uri, content);
                    operationLocation = response.Headers.ToString();

                    // Display the JSON response.
                    //Console.WriteLine("\nResponse:\n\n{0}\n",
                    //    JToken.Parse(contentString).ToString());
                    // Get the JSON response.
                    result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                // Display the JSON response.
            }
            catch (Exception e)
            {
                result = e.Message + "this is the error";
            }
            return result;
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            // Open a read-only file stream for the specified file.
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                // Read the file's contents into a byte array.
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }


    }
}


