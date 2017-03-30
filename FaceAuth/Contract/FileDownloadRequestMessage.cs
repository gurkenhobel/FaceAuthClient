using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAuth.Contract
{
    class FileDownloadRequestMessage : Message
    {
        public string filename { get; set; }

        public FileDownloadRequestMessage()
        {
            MessageType = MessageType.FileDownloadRequest;
        }
    }
}
