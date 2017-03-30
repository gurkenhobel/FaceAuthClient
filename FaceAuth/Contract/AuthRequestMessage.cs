using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAuth.Contract
{
    class AuthRequestMessage:Message
    {
        public string Image { get; set; }
        public AuthRequestMessage()
        {
            MessageType=MessageType.AuthRequest;
        }

    }
}
