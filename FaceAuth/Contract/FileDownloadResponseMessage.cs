using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceAuth.Model;

namespace FaceAuth.Contract
{
    class FileDownloadResponseMessage : Message
    {
        public bool success { get; set; }
        public string data { get; set; }


        public FileDownloadResponseMessage()
        {
            MessageType = MessageType.FileDownloadResponse;
        }
    }
}
