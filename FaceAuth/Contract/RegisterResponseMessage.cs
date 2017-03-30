using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAuth.Contract
{
    class RegisterResponseMessage: Message
    {
        public bool success { get; set; }

        public RegisterResponseMessage()
        {
            MessageType = MessageType.RegisterResponse;
        }
    }
}
