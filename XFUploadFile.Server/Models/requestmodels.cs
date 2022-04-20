using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XFUploadFile.Server.Models
{
    //public class requestmodels
    //{
    //    public requestmodels()
    //    {
    //    }
    //}

    public class requestData
    {
        public string filetype { get; set; }
        //public byte[] Data { get; set; }
        public Stream stream { get; set; }
    }

    public class requestfaceData
    {
        public string filetype { get; set; }
        public byte[] Data1 { get; set; }
        public byte[] Data2 { get; set; }
    }
    public class FaceRectangle
    {
        public int top { get; set; }
        public int left { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Emotion
    {
        public double anger { get; set; }
        public double contempt { get; set; }
        public double disgust { get; set; }
        public double fear { get; set; }
        public double happiness { get; set; }
        public double neutral { get; set; }
        public double sadness { get; set; }
        public double surprise { get; set; }
    }

    public class FaceAttributes
    {

        public Emotion emotion { get; set; }
    }

    public class faceresponse
    {
        public string faceId { get; set; }
        public FaceRectangle faceRectangle { get; set; }
        public FaceAttributes faceAttributes { get; set; }
    }
    //[{"faceId":"eb72a629-c2d2-4f49-804b-f7367e7999bf","faceRectangle":{"top":41,"left":100,"width":87,"height":87},"faceAttributes":{"emotion":{"anger":0.0,"contempt":0.0,"disgust":0.0,"fear":0.0,"happiness":0.0,"neutral":0.997,"sadness":0.002,"surprise":0.0}}}]


    public class VerifyRequestModel
    {

        public string faceId1 { get; set; }

        public string faceId2 { get; set; }
    }

    public class verifyResponceModel
    {
        public bool isIdentical { get; set; }
        public double confidence { get; set; }
    }

    public class localfilerequest
    {
        public string imageFilePath1 { get; set; }

        public string imageFilePath2 { get; set; }
    }
    

}
