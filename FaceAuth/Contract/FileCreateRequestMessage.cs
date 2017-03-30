using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceAuth.Annotations;

namespace FaceAuth.Contract
{
    class FileCreateRequestMessage: Message
    {
        public string name { get; set; }

        public bool encrypted { get; set; }
        public string data { get; set; }

        public FileCreateRequestMessage()
        {

            MessageType = MessageType.FileCreateRequest;
        }
    }
}
