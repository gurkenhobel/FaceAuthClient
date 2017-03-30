using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAuth.Contract
{
    class FileDeleteRequestMessage : Message
    {
        public string filename { get; set; }

        public FileDeleteRequestMessage()
        {
            MessageType = MessageType.FileDeleteRequest;
        }
    }
}
