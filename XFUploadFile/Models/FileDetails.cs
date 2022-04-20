using System;
namespace XFUploadFile.Models
{
    //public class FileDetails
    //{
    //    public FileDetails()
    //    {
    //    }
    //}

    public class requestData
    {
        public string filetypeimage { get; set; }
        public string filetypevideo { get; set; }
        public byte[] Data { get; set; }
        public byte[] imgData { get; set; }
        //public Stream stream { get; set; }
    }
}
