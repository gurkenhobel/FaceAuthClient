using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAuth.Contract
{
    class AuthResponseMessage:Message
    {
        public string name { get; set; }
        public bool success { get; set; }
        public AuthResponseMessage()
        {
            MessageType = MessageType.AuthResponse;
        }
    }
}
